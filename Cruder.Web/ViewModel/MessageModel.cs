using Cruder.Core;
using Cruder.Resource;

namespace Cruder.Web.ViewModel
{
    public class MessageModel
    {
        public MessageType Type { get; set; }
        public string Content { get; set; }

        public MessageModel()
        {
            this.Type = ViewModel.MessageType.Information;
            this.Content = string.Empty;
        }

        public MessageModel(string content, MessageType type)
        {
            this.Type = type;
            this.Content = content;
        }

        public MessageModel(Result result)
        {
            if (result.HasError)
            {
                this.Type = MessageType.Failure;
                this.Content = ResourceManager.GetString("Web.ViewModel.MessageModel.ErrorContent"); //An error has occurred while your request was being processed.

                if (result is Result<int>)
                {
                    var genericResult = (Result<int>)result;
                    if (genericResult.Data > 0)
                    {
                        this.Content += string.Format(" {0} : <b>{1}</b>.", ResourceManager.GetString("Web.ViewModel.MessageModel.ErrorCodeExtensionContent"), genericResult.Data);
                        
                    }
                }

                this.Content += string.Format("<br /><br />{0} : {1}", ResourceManager.GetString("Web.ViewModel.MessageModel.ErrorDetailsContent"), result.Message);
            }
            else
            {
                this.Type = MessageType.Information;

                if (string.IsNullOrEmpty(result.Message))
                {
                    this.Content = ResourceManager.GetString("Web.ViewModel.MessageModel.SuccessfullyContent"); //Your request has been processed successfully.
                }
                else
                {
                    this.Content = result.Message;
                }
            }
        }
    }

    public enum MessageType
    {
        Failure = -1,
        Warning = 0,
        Information = 1,
        Success = 2
    }
}
