using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.IO;

namespace jaramillo.cl.Common
{
    /// <summary>
    /// Write error on console referencing the method caller and the file
    /// </summary>
    public class ErrorWriter
    {
        /// <summary>
        /// Print a Invalid Arguments error on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        public static void InvalidArgumentsError([CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int? line = null)
        {
            var callerFile = Path.GetFileName(callerFilePath);
            string lineStr = line.ToString() ?? "LINE NOT FOUND";

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  Invalid Arguments\nOn File  :  {callerFile}\nOn Line  :  {lineStr}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

        /// <summary>
        /// Print the message of an Exception on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        /// <param name="exception"> Exception from where to extract the message </param>
        public static void ExceptionError(Exception exception, [CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null)
        {
            // Get stack trace for the exception with source file information
            var stackTrace = new StackTrace(exception, true);
            // Get the top stack frame
            var frame = stackTrace.GetFrame(0);
            // Get the line number from the stack frame
            int line = frame.GetFileLineNumber();

            var callerFile = Path.GetFileName(callerFilePath);
            string lineStr = line.ToString() ?? "LINE NOT FOUND";


            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  {exception.Message}\nOn File  :  {callerFile}\nOn Line  :  {lineStr}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

        /// <summary>
        /// Print a custom message on the Debug Console, with the caller name and the file of it
        /// <para> callerName and callerFilePath are filled automatically </para>
        /// </summary>
        /// <param name="message"> Error message to write </param>
        public static void CustomError(string message, [CallerMemberName] string callerName = "", [CallerFilePath]string callerFilePath = null, [CallerLineNumber]int? line = null)
        {
            var callerFile = Path.GetFileName(callerFilePath);
            string lineStr = line.ToString() ?? "LINE NOT FOUND";

            Debug.WriteLine("----------------------------------------------------------------");
            Debug.WriteLine($"ERROR in {callerName}()  :  {message}\nOn File  :  {callerFile}\nOn Line  :  {lineStr}");
            Debug.WriteLine("----------------------------------------------------------------");
        }

    }
}