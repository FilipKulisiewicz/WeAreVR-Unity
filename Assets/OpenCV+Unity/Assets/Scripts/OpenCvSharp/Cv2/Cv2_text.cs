// using System;
// using System.Collections.Generic;

// using OpenCvSharp;

// namespace OpenCvSharp
// {

//     // ReSharper disable InconsistentNaming
//     /// <summary>
//     /// Recognize text using the tesseract-ocr API.
//     /// 
//     /// Takes image on input and returns recognized text in the output_text parameter.
//     /// Optionallyprovides also the Rects for individual text elements found(e.g.words), 
//     /// and the list of those text elements with their confidence values.
//     /// </summary>
//     static partial class Cv2
//     {
//         /// <summary>
//         /// Recognize text using the tesseract-ocr API.
//         /// Takes image on input and returns recognized text in the output_text parameter.
//         /// Optionally provides also the Rects for individual text elements found(e.g.words), 
//         /// and the list of those text elements with their confidence values.
//         /// </summary>
//         /// <param name="image">Input image CV_8UC1 or CV_8UC3</param>
//         /// <param name="outputText">Output text of the tesseract-ocr.</param>
//         /// <param name="componentRects">If provided the method will output a list of Rects for the individual 
//         /// text elements found(e.g.words or text lines).</param>
//         /// <param name="componentTexts">If provided the method will output a list of text strings for the 
//         /// recognition of individual text elements found(e.g.words or text lines).</param>
//         /// <param name="componentConfidences">If provided the method will output a list of confidence values 
//         /// for the recognition of individual text elements found(e.g.words or text lines).</param>
//         /// <param name="componentLevel">OCR_LEVEL_WORD (by default), or OCR_LEVEL_TEXT_LINE.</param>
//         public static void Run(
//             Mat image,
//             out string outputText,
//             out Rect[] componentRects,
//             out string?[] componentTexts,
//             out float[] componentConfidences)
//         {
//             if (image is null)
//                 throw new ArgumentNullException(nameof(image));
//             image.ThrowIfDisposed();

//             using var outputTextString = new StdString();
//             using var componentRectsVector = new VectorOfRect();
//             using var componentTextsVector = new VectorOfString();
//             using var componentConfidencesVector = new VectorOfFloat();
//             NativeMethods.HandleException(
//                 NativeMethods.text_OCRTesseract_run1(
//                     ptr,
//                     image.CvPtr,
//                     outputTextString.CvPtr,
//                     componentRectsVector.CvPtr,
//                     componentTextsVector.CvPtr,
//                     componentConfidencesVector.CvPtr,
//                     (int) 0));          //Word	0   TextLine	1
//             outputText = outputTextString.ToString();
//             componentRects = componentRectsVector.ToArray();
//             componentTexts = componentTextsVector.ToArray();
//             componentConfidences = componentConfidencesVector.ToArray();
//         }
//     }
// }