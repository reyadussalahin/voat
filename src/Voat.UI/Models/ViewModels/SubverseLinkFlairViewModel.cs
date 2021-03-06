#region LICENSE

/*
    
    Copyright(c) Voat, Inc.

    This file is part of Voat.

    This source file is subject to version 3 of the GPL license,
    that is bundled with this package in the file LICENSE, and is
    available online at http://www.gnu.org/licenses/gpl-3.0.txt;
    you may not use this file except in compliance with the License.

    Software distributed under the License is distributed on an
    "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express
    or implied. See the License for the specific language governing
    rights and limitations under the License.

    All Rights Reserved.

*/

#endregion LICENSE

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Voat.Models.ViewModels
{
    public class SubverseLinkFlairViewModel
    {
        [StringLength(50, ErrorMessage = "Flair label is limited to 50 characters.")]
        [DisplayName("Flair label")]
        public string Label { get; set; }

        public string Subversename { get; set; }

        [StringLength(50, ErrorMessage = "Flair CSS class name is limited to 50 characters.")]
        [DisplayName("Flair CSS class name")]
        public string CssClass { get; set; }
    }
}
