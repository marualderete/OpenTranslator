﻿using System.ComponentModel.DataAnnotations;

namespace OpenTranslator.Models.Input
{
    public class LanguageInput
    {
        [UIHint("Hidden")]
        public string Id { get; set; }

        [Required]
		[MaxLength(2)]
        public string LanguageCode { get; set; }
		[Required]
        public string LanguageName { get; set; }
    }
}