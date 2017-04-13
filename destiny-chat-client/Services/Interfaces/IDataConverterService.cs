using System.Windows.Documents;
using ComboMessage = destiny_chat_client.Views.Components.ComboMessage;
using Message = destiny_chat_client.Models.Message;

namespace destiny_chat_client.Services.Interfaces
{
    /// <summary>
    ///     A conversion from the (destiny.gg chat server response) 
    ///     to the (chat window data model)
    /// </summary>
    public interface IDataConverterService
    {
        /// <summary>
        ///     The conversion from message to the currently used paragraph
        /// </summary>
        Paragraph MessageToParagraph(Message message);

        /// <summary>
        ///     The last message stored in the chat (used in parsing)
        /// </summary>
        Views.Components.Message LastMessage { get; set; }

        /// <summary>
        ///     The combo message used for modifying a emote combo
        /// </summary>
        ComboMessage Combo { get; set; }
    }
}