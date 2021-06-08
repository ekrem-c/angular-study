namespace Platform.Common
{
    public class ModelErrorMessage
    {
        public ModelErrorMessage(ModelError error)
        {
            Error = error;
        }

        /// <summary>
        ///     Gets the Error Information.
        /// </summary>
        public ModelError Error { get; set; }
    }
}