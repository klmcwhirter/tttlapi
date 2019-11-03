namespace tttlapi.Models
{
    /// <summary>
    /// Desribes potential locations on a Tic Tac Toe board
    /// </summary>
    public enum BoardLocation
    {
        /// <summary>
        /// A corner of the board
        /// </summary>
        Corner,

        /// <summary>
        /// A side of the board
        /// </summary>
        Side,

        /// <summary>
        /// The center of the board
        /// </summary>
        Center,

        /// <summary>
        /// Any location on the board
        /// </summary>
        Any
    }
}
