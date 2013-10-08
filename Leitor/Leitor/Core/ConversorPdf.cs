﻿using System;
using iTextSharp.text.pdf;

namespace Leitor.Core
{
    public class ConversorPdf
    {
        /// BT = Beginning of a text object operator
        /// ET = End of a text object operator
        /// Td move to the start of next line
        ///  5 Ts = superscript
        /// -5 Ts = subscript

        #region Fields

        #region NumberOfCharsToKeep

        /// <summary>
        /// The number of characters to keep, when extracting text.
        /// </summary>
        private const int NumberOfCharsToKeep = 15;

        #endregion

        #endregion

        #region ExtrairTexto

        /// <summary>
        /// Extracts a text from a PDF file.
        /// </summary>
        /// <param name="nomeArquivo">the full path to the pdf file.</param>
        /// <returns>the extracted text</returns>
        public static string ExtrairTexto(string nomeArquivo)
        {
            string texto = String.Empty;
            // Create a reader for the given PDF file
            PdfReader reader = new PdfReader(nomeArquivo);
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                texto = ExtractTextFromPdfBytes(reader.GetPageContent(page)) + " ";
            }
            reader.Dispose();

            //tentar pegar pelo segundo jeito se o outro nao der certo
            if(String.IsNullOrWhiteSpace(texto) || texto.Length < 10)
            {
                texto = PdfToText.ExtrairTextoDoPdf(nomeArquivo);
            }
            return texto;
        }

        #endregion

        #region ExtractTextFromPDFBytes

        /// <summary>
        /// This method processes an uncompressed Adobe (text) object
        /// and extracts text.
        /// </summary>
        /// <param name="input">uncompressed</param>
        /// <returns></returns>
        private static string ExtractTextFromPdfBytes(byte[] input)
        {
            if (input == null || input.Length == 0) return "";

            try
            {
                string resultString = "";

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;

                // Flag showing if the next character is literal
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;

                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;

                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[NumberOfCharsToKeep];
                for (int j = 0; j < NumberOfCharsToKeep; j++) previousCharacters[j] = ' ';


                for (int i = 0; i < input.Length; i++)
                {
                    char c = (char) input[i];

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] {"TD", "Td"}, previousCharacters))
                            {
                                resultString += "\n\r";
                            }
                            else
                            {
                                if (CheckToken(new string[] {"'", "T*", "\""}, previousCharacters))
                                {
                                    resultString += "\n";
                                }
                                else
                                {
                                    if (CheckToken(new string[] {"Tj"}, previousCharacters))
                                    {
                                        resultString += " ";
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(new string[] {"ET"}, previousCharacters))
                        {
                            inTextObject = false;
                            resultString += " ";
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what.
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~')) ||
                                                ((c >= 128) && (c < 255)))
                                            {
                                                resultString += c.ToString();
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for
                    // when we have to go back for a checking
                    for (int j = 0; j < NumberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[NumberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] {"BT"}, previousCharacters))
                    {
                        resultString += "\r\n";
                        inTextObject = true;
                    }
                }
                return resultString;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region CheckToken

        /// <summary>
        /// Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name="search">the searched token</param>
        /// <param name="recent">the recent character array</param>
        /// <returns></returns>
        private static bool CheckToken(string[] tokens, char[] recent)
        {
            foreach (string token in tokens)
            {
                if ((recent[NumberOfCharsToKeep - 3] == token[0]) &&
                    (recent[NumberOfCharsToKeep - 2] == token[1]) &&
                    ((recent[NumberOfCharsToKeep - 1] == ' ') ||
                     (recent[NumberOfCharsToKeep - 1] == 0x0d) ||
                     (recent[NumberOfCharsToKeep - 1] == 0x0a)) &&
                    ((recent[NumberOfCharsToKeep - 4] == ' ') ||
                     (recent[NumberOfCharsToKeep - 4] == 0x0d) ||
                     (recent[NumberOfCharsToKeep - 4] == 0x0a))
                    )
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}