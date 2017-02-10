﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voat.Data.Models;
using Voat.Domain.Command;
using Voat.Utilities;

namespace Voat.Tests.CommandTests
{
    [TestClass]
    public class VoteValueCommandTests : BaseUnitTest
    {
        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("Command.Vote")]
        [TestCategory("Command.Comment.Vote")]
        [TestCategory("Command.Comment.Vote.VoteValue")]
        public async Task Vote_NonRestricted()
        {
            //Anon voting should not count towards target
            await VerifyVoteStatus("TestUser25", "unit", Domain.Models.ContentType.Submission, 1, 1);
            await VerifyVoteStatus("TestUser25", "unit", Domain.Models.ContentType.Submission, -1, -1);

            await VerifyVoteStatus("TestUser26", "unit", Domain.Models.ContentType.Comment, 1, 1);
            await VerifyVoteStatus("TestUser26", "unit", Domain.Models.ContentType.Comment, -1, -1);
        }

        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("Command.Vote")]
        [TestCategory("Command.Comment.Vote")]
        [TestCategory("Command.Comment.Vote.VoteValue")]
        public async Task Vote_Anon()
        {
            //Anon voting should not count towards target
            await VerifyVoteStatus("TestUser21", "anon", Domain.Models.ContentType.Submission, 1, 0);
            await VerifyVoteStatus("TestUser21", "anon", Domain.Models.ContentType.Submission, -1, 0);

            await VerifyVoteStatus("TestUser11", "anon", Domain.Models.ContentType.Comment, 1, 0);
            await VerifyVoteStatus("TestUser11", "anon", Domain.Models.ContentType.Comment, -1, 0);
        }

        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("Command.Vote")]
        [TestCategory("Command.Comment.Vote")]
        [TestCategory("Command.Comment.Vote.VoteValue")]
        public async Task Vote_Private()
        {
            //Anon voting should not count towards target
            await VerifyVoteStatus("TestUser22", "private", Domain.Models.ContentType.Submission, 1, 0);
            await VerifyVoteStatus("TestUser22", "private", Domain.Models.ContentType.Submission, -1, 0);

            await VerifyVoteStatus("TestUser23", "private", Domain.Models.ContentType.Comment, 1, 0);
            await VerifyVoteStatus("TestUser23", "private", Domain.Models.ContentType.Comment, -1, 0);
        }

        [TestMethod]
        [TestCategory("Command")]
        [TestCategory("Command.Vote")]
        [TestCategory("Command.Comment.Vote")]
        [TestCategory("Command.Comment.Vote.VoteValue")]
        public async Task Vote_MinCCP()
        {
            //Anon voting should not count towards target
            await VerifyVoteStatus("TestUser24", "minCCP", Domain.Models.ContentType.Submission, 1, 0);

            await VerifyVoteStatus("TestUser25", "minCCP", Domain.Models.ContentType.Comment, 1, 0);
        }
        private async Task VerifyVoteStatus(string userToPost, string subverse, Domain.Models.ContentType contentType, int voteStatus, int voteValue)
        {
            int id = 0;
            string userName = "";
            //Create submission
            TestHelper.SetPrincipal(userToPost);
            var cmd = new CreateSubmissionCommand(new Domain.Models.UserSubmission() { Subverse = subverse, Title = "VerifyVoteStatus Test Submission in " + subverse });
            var response = cmd.Execute().Result;
            Assert.AreEqual(Status.Success, response.Status, response.Message);
            var submission = response.Response;

            //voting username
            userName = "User100CCP";
            if (contentType == Domain.Models.ContentType.Submission)
            {
                id = submission.ID;
                TestHelper.SetPrincipal(userName);
                var voteSubmissionCommand = new SubmissionVoteCommand(id, voteStatus, Guid.NewGuid().ToString());
                var voteSubmissionResponse = await voteSubmissionCommand.Execute();
                Assert.IsNotNull(voteSubmissionResponse, "Expecting non-null submission vote command");

                //verify in db
                using (var db = new voatEntities())
                {
                    var record = db.SubmissionVoteTrackers.Where(x => x.SubmissionID == id && x.UserName == userName).FirstOrDefault();
                    Assert.IsNotNull(record, "Expecting a vote record");
                    Assert.AreEqual(voteStatus, record.VoteStatus);
                    Assert.AreEqual(voteValue, record.VoteValue);
                }

            }
            else if (contentType == Domain.Models.ContentType.Comment)
            {
                //Create comment 
                var cmdComment = new CreateCommentCommand(submission.ID, null, $"VerifyVoteStatus Test Submission in {subverse} - {Guid.NewGuid().ToString()}");
                var responseComment = await cmdComment.Execute();
                Assert.AreEqual(Status.Success, responseComment.Status, responseComment.Message);
                id = responseComment.Response.ID;

                TestHelper.SetPrincipal(userName);
                var voteCommentCommand = new CommentVoteCommand(id, voteStatus, Guid.NewGuid().ToString());
                var voteCommentResponse = await voteCommentCommand.Execute();
                Assert.IsNotNull(voteCommentResponse, "Expecting non-null submission vote command");

                //verify in db
                using (var db = new voatEntities())
                {
                    var record = db.CommentVoteTrackers.Where(x => x.CommentID == id && x.UserName == userName).FirstOrDefault();
                    Assert.IsNotNull(record, "Expecting a vote record");
                    Assert.AreEqual(voteStatus, record.VoteStatus);
                    Assert.AreEqual(voteValue, record.VoteValue);
                }
            }
        }
    }
}
