namespace KBWebApplication.Dto
{
	public sealed class AnswerDto(string answer, List<string>? links)
    {
		public string Answer { get; set; } = answer;
        public List<string>? Links { get; set; } = links;
    }
}

