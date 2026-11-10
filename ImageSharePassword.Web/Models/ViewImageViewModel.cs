using ImageSharePassword.Data;

namespace ImageSharePassword.Web.Models
{
    public class ViewImageViewModel
    {
        public Image Image { get; set; }
        public bool Show { get; set; } = false;
        public string Message { get; set; }
    }
}