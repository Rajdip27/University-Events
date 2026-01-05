namespace UniversityEvents.Application.CommonModel;

public class EmailMessage
{
    public List<string> To { get; set; } = new List<string>();
    public List<string> CC { get; set; }
    public List<string> BCC { get; set; }
    public string Subject { get; set; } = "";
    public string HtmlFilePath { get; set; } // optional HTML file
    public string Message { get; set; }      // optional plain message
    public List<string> Attachments { get; set; } // optional files
}
