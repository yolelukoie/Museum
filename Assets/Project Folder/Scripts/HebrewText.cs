using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;


// This class will reverse a string or a multiline string
// from https://github.com/liron-navon/ILText
public static class HebrewText
{
    /* will Reverse a multiline or normal string 
         * @s the string to reverse
        **/
    public static string ReverseString(string s)
    {

        String[] lines = s.Split("\n\r".ToCharArray(), StringSplitOptions.None);

        String[] newStringArr = new string[lines.Length];

        for (int i = 0; i < lines.Length; i++)
        {
            newStringArr[i] = ReverseLine(lines[i]);
        }

        return String.Join("\n", newStringArr);
    }

    /* will Reverse a single line string
     * @s the string to reverse
    **/
    public static string ReverseLine(string s)
    {

        char[] charArray = s.ToCharArray();
        char[] reversedArray = new char[charArray.Length];

        //allocate lists to hold latin chars and indexes
        List<int> indexes = new List<int>();
        List<char> latinChars = new List<char>();

        bool wasLastCharLatin = false;

        for (int i = 0; i < charArray.Length; i++)
        {

            //select a char in a reversed order
            char c = charArray[charArray.Length - 1 - i];

            // add latin chars to seperate lists
            if (Regex.IsMatch(c.ToString(), "^[a-zA-Z0-9]*$"))
            {
                wasLastCharLatin = true;
                latinChars.Add(c);
                indexes.Add(i);
            }
            else
            {
                wasLastCharLatin = false;
                if (indexes.Count > 0)
                {
                    //  iterate the numbers in a reversed order
                    for (int j = 0; j < indexes.Count; j++)
                    {
                        int a = indexes[j];
                        reversedArray[a] = latinChars[indexes.Count - 1 - j];
                    }
                    //clear the numbers lists
                    indexes.Clear();
                    latinChars.Clear();
                }

                // add the next char
                reversedArray[i] = c;
            }
        }

        if (indexes.Count > 0)
        {
            for (int j = 0; j < indexes.Count; j++)
            {
                int a = indexes[j];
                reversedArray[a] = latinChars[indexes.Count - 1 - j];
            }
            //clear the numbers lists
            indexes.Clear();
            latinChars.Clear();
        }

        // Array.Reverse(charArray);
        return new string(reversedArray);
    }
}
