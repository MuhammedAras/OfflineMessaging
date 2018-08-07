using System.ComponentModel.DataAnnotations;


namespace OfflineMessaging.Models
{
    public class MessageBindingModel
    {
        
            [Required]
            [Display(Name = "Receiver UserName")]
            public string ReceiverUserName { get; set; }

            [Required]
            [Display(Name = "Message")]
            public string Message { get; set; }
        
    }
}