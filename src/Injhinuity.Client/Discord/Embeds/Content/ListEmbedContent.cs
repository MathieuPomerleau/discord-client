using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Injhinuity.Client.Core.Resources;

namespace Injhinuity.Client.Discord.Embeds.Content
{
    public class ListEmbedContent : IReactionEmbedContent
    {
        private readonly int _fieldsPerPage;
        private readonly InjhinuityEmbedField[] _fields;
        private readonly EmbedBuilder _embedBuilder;

        private bool IsSinglePage => _fields.Length <= _fieldsPerPage;
        private bool IsAtMaxPage => (CurrentPage + 1) * _fieldsPerPage >= _fields.Length;
        private bool IsAtMinPage => CurrentPage == 0;
        
        private int _oldPage = -1;
        public int CurrentPage { get; private set; } = 0;

        public ListEmbedContent(int fieldsPerPage, IEnumerable<InjhinuityEmbedField>? fields, EmbedBuilder embedBuilder)
        {
            _fieldsPerPage = fieldsPerPage;
            _fields = fields?.ToArray() ?? new InjhinuityEmbedField[] { };
            _embedBuilder = embedBuilder;
        }

        public EmbedBuilder Get()
        {
            if (_oldPage == CurrentPage)
                return _embedBuilder;
            
            var start = CurrentPage * _fieldsPerPage;
            var end = Math.Min(start + _fieldsPerPage, _fields.Length);

            _embedBuilder.Fields.Clear();

            if (_fields.Length == 0)
            {
                _embedBuilder.AddField(CommonResources.FieldTitleEmpty, CommonResources.FieldValueEmpty);
            }
            else
            {
                foreach (var field in _fields[start..end])
                    _embedBuilder.AddField(field.Name, field.Value, field.Inline);
            }

            _oldPage = CurrentPage;
            return _embedBuilder;
        }

        public void NextPage()
        {
            if (IsSinglePage || IsAtMaxPage)
                return;

            _oldPage = CurrentPage;
            CurrentPage += 1;
        }

        public void PreviousPage()
        {
            if (IsSinglePage || IsAtMinPage)
                return;

            _oldPage = CurrentPage;
            CurrentPage -= 1;
        }
    }
}
