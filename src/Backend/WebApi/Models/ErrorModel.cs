namespace WebApi.Models
{
    public class ErrorModel
    {
        public IDictionary<string, ICollection<string>> Errors { get; set; }
    }
}
