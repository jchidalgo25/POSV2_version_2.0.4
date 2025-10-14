using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.Models
{
    class Barcode
    {
        const int EscChar = 126;
            const char EscCharStr = '~';
            const string F1_ENCODE="~f";

            const string StartAOptimizer="~g";
            const string StartBOptimizer="~h";
            const string StartCOptimizer="~i";

            const string Shift2AOptimizer="~e";
            const string Shift2BOptimizer="~d";
            const string Shift2COptimizer="~c";

            const int CodesetA = 1;
            const int CodesetB=2;
            const int CodesetC=3;

            const int NextA=1;
            const int NextB=2;

            const int NotFound=-1;

            const int MaxReturnStringSize = 70;

            string _codebefore;

            public bool setCode(string code)
            {
                _codebefore = code;
                return true;
            }
        

            public string encodeString(string _stringIn, bool useF1Code = false)
            {
                int                 len1    = (_stringIn + '.').Length - 1; // handle trailing spaces so make sure len1 includes specified spaces
                bool                cont = true;
                int                 tichr = 0;
                int                 codeset = CodesetB;
                int                 fcn4 = 1;
                int                 tcchr;
                int                 chktot = 0;
                int                 val;
                int                 charVal;
                int                 chrcnt = 0;
                int                 rtnint;
                string       textcvt;
                string       result="";
                bool             shift = false;
                int                 outputCnt = 0;
                ;
                if (len1 < 1)
                    return _stringIn;

                // check the character string and optimize the structure
                textcvt = this.optimizeCodesets(_stringIn,len1-1, useF1Code);

                // map the characters
                while (cont)
                {
                    charVal = textcvt.Substring(tichr, 1).ToCharArray()[0];
                    if ((charVal == EscChar) && (textcvt.Substring(tichr,1).ToCharArray()[0] != 0))   // tilda character 126
                    {
                        tichr++;
                        charVal = textcvt.Substring(tichr,1).ToCharArray()[0];

                        // make the val variable equal to the character after the /
                        if (charVal == EscChar)
                            val = 95; // if the character is a esckey then make the value a 95
                        else
                            val = (charVal < 0 || charVal > 105) ? NotFound : charVal + 1; // convert to index

                        if (val != NotFound)
                            result += this.index2OutStr(val);
                        else
                            result += ((char)(63)).ToString() + ((char)(118)).ToString() + ((char)(63)).ToString();

                        if (chrcnt == 0)
                            chktot += val - 1;
                        else
                            chktot += chrcnt * (val - 1);

                        chrcnt++;
                        outputCnt += 3;

                        // handle FNC4
                        if (((charVal == 100) && (codeset == CodesetB)) || ((charVal == 101) && (codeset == CodesetA)))
                        {
                            if (fcn4 < 4)
                                fcn4++;
                            else
                                fcn4 = 1;
                        }
                        else
                        {
                            switch(charVal)
                            {
                                case 101:
                                case 103: codeset = CodesetA; break;
                                case 100:
                                case 104: codeset = CodesetB; break;
                                case  99:
                                case 105: codeset = CodesetC; break;
                            }
                        }
                        tichr++;
                    }
                    else
                    {
                        if ((codeset == CodesetC) && (charVal >= 48) && (charVal <= 57))
                        {
                            rtnint = this.codesetCSearch(textcvt, tichr);
                            if (rtnint != -1)
                            {
                                result += this.index2OutStr(rtnint);

                                chktot += chrcnt * (rtnint - 1);
                                chrcnt++;
                                outputCnt += 3;
                            }
                            else
                            {
                                result += ((char)(63)).ToString() + ((char)(99)).ToString() + ((char)(63)).ToString();
                                outputCnt += 3;
                            }
                            tichr += 2;
                        }
                        else
                        {
                            if (fcn4 > 1)
                            {
                                if (fcn4 == 4)
                                    tcchr = textcvt.Substring(tichr,1).ToCharArray()[0] + 128;
                                else
                                    tcchr = textcvt.Substring(tichr,1).ToCharArray()[0];

                                if (fcn4 == 2)
                                    fcn4 = 1;
                                else
                                    fcn4 = 3;
                            }
                            else
                                tcchr = textcvt.Substring(tichr, 1).ToCharArray()[0];

                            tichr++;
                            if (shift)
                            {
                                if (codeset == CodesetA)
                                    rtnint = (tcchr <= 31 || tcchr > 127) ? NotFound : tcchr - 31; // is tcchr in codeset B?
                                else
                                {
                                    rtnint = (tcchr < 0 || tcchr > 127) ? NotFound : (tcchr <= 31 ) ? tcchr + 65 : tcchr - 31; // is tcchr in codeset a
                                }
                                shift = false;
                            }
                            else
                            {
                                if (codeset == CodesetA)
                                {
                                    rtnint = (tcchr < 0 || tcchr > 127) ? 96 : (tcchr <= 31) ? tcchr + 65 : tcchr - 31; // is tcchr in codeset a
                                }
                                else
                                {
                                    rtnint = (tcchr <= 31 || tcchr > 127) ? NotFound : tcchr - 31; // is tcchr in codeset B?
                                }
                            }
                            if (rtnint != NotFound)
                            {
                                result += this.index2OutStr(rtnint);

                                chktot += chrcnt * (rtnint - 1);
                                chrcnt++;
                                outputCnt += 3;
                            }
                            else
                            {
                                result += (char)(63) + (char)(98) + (char)(63);
                                outputCnt += 3;
                            }
                            if ((charVal == 98) && ((codeset == CodesetA) || (codeset == CodesetB)))
                                shift = true;
                        }
                    }

                    if (textcvt.Length <= tichr)
                        cont = false;
                    else
                        if (textcvt.Substring(tichr, 1).ToCharArray()[0] <= 1)
                            cont = false;
                    
                }

                val = chktot % 103;
                if ((codeset == CodesetA) && (val == 91))  // ESc
                {
                    result += this.index2OutStr(101); // shift to codeset b
                    chktot += chrcnt * (100);
                    chrcnt++;

                    result += this.index2OutStr(74);
                    chktot += chrcnt * (73);
                    chrcnt++;
                    outputCnt += 6;

                    val = chktot % 103;
                }

                result += this.index2OutStr(1 + val);
                // add stop character
                result += "\')!1";  // num2char(39) + num2char(41) + num2char(33) + num2char(49)

                if (outputCnt > MaxReturnStringSize - 7) // 3 start + 4 stop
                    throw new Exception("Erroooor");
                return result;
            }


            protected int codesetCSearch(string _stringIn, int _startPos)
            {
                int c1 = _stringIn.Substring(_startPos,1).ToCharArray()[0] - 48;
                int c2 = (_stringIn.Length-1>_startPos?_stringIn.Substring(_startPos + 1, 1).ToCharArray()[0] - 48:-1);
                ;
                if (c1 < 0 || c1 > 9)
                    return NotFound;

                if (c2 < 0 || c2 > 9)
                    return NotFound;

                return 10 * c1 + c2 + 1;
            }



            protected string index2OutStr(int _idx)
            {
                ;
                switch(_idx)
                {
                    case 106:   return (((char)(37)).ToString() + ((char)(34)).ToString() + ((char)(42)).ToString()).ToString();
                    case 105:   return (((char)(37)).ToString() + ((char)(34)).ToString() + ((char)(36)).ToString()).ToString();
                    case 104:   return (((char)(37)).ToString() + ((char)(36)).ToString() + ((char)(34)).ToString()).ToString();
                    case 103:   return (((char)(45)).ToString() + ((char)(33)).ToString() + ((char)(41)).ToString()).ToString();
                    case 102:   return (((char)(41)).ToString() + ((char)(33)).ToString() + ((char)(45)).ToString()).ToString();
                    case 101:   return (((char)(33)).ToString() + ((char)(45)).ToString() + ((char)(41)).ToString()).ToString();
                    case 100:   return (((char)(33)).ToString() + ((char)(41)).ToString() + ((char)(45)).ToString()).ToString();
                    case  99:   return (((char)(45)).ToString() + ((char)(35)).ToString() + ((char)(33)).ToString()).ToString();
                    case  98:   return (((char)(45)).ToString() + ((char)(33)).ToString() + ((char)(35)).ToString()).ToString();
                    case  97:   return (((char)(33)).ToString() + ((char)(47)).ToString() + ((char)(33)).ToString()).ToString();
                    case  96:   return (((char)(33)).ToString() + ((char)(45)).ToString() + ((char)(35)).ToString()).ToString();
                    case  95:   return (((char)(35)).ToString() + ((char)(33)).ToString() + ((char)(45)).ToString()).ToString();
                    case  94:   return (((char)(33)).ToString() + ((char)(35)).ToString() + ((char)(45)).ToString()).ToString();
                    case  93:   return (((char)(33)).ToString() + ((char)(33)).ToString() + ((char)(47)).ToString()).ToString();
                    case  92:   return (((char)(45)).ToString() + ((char)(37)).ToString() + ((char)(37)).ToString()).ToString();
                    case  91:   return (((char)(37)).ToString() + ((char)(45)).ToString() + ((char)(37)).ToString()).ToString();
                    case  90:   return (((char)(37)).ToString() + ((char)(37)).ToString() + ((char)(45)).ToString()).ToString();
                    case  89:   return (((char)(46)).ToString() + ((char)(34)).ToString() + ((char)(33)).ToString()).ToString();
                    case  88:   return (((char)(46)).ToString() + ((char)(33)).ToString() + ((char)(34)).ToString()).ToString();
                    case  87:   return (((char)(45)).ToString() + ((char)(34)).ToString() + ((char)(34)).ToString()).ToString();
                    case  86:   return (((char)(34)).ToString() + ((char)(46)).ToString() + ((char)(33)).ToString()).ToString();
                    case  85:   return (((char)(34)).ToString() + ((char)(45)).ToString() + ((char)(34)).ToString()).ToString();
                    case  84:   return (((char)(33)).ToString() + ((char)(46)).ToString() + ((char)(34)).ToString()).ToString();
                    case  83:   return (((char)(34)).ToString() + ((char)(34)).ToString() + ((char)(45)).ToString()).ToString();
                    case  82:   return (((char)(34)).ToString() + ((char)(33)).ToString() + ((char)(46)).ToString()).ToString();
                    case  81:   return (((char)(33)).ToString() + ((char)(34)).ToString() + ((char)(46)).ToString()).ToString();
                    case  80:   return (((char)(35)).ToString() + ((char)(45)).ToString() + ((char)(33)).ToString()).ToString();
                    case  79:   return (((char)(40)).ToString() + ((char)(33)).ToString() + ((char)(34)).ToString()).ToString();
                    case  78:   return (((char)(45)).ToString() + ((char)(41)).ToString() + ((char)(33)).ToString()).ToString();
                    case  77:   return (((char)(38)).ToString() + ((char)(33)).ToString() + ((char)(36)).ToString()).ToString();
                    case  76:   return (((char)(40)).ToString() + ((char)(34)).ToString() + ((char)(33)).ToString()).ToString();
                    case  75:   return (((char)(36)).ToString() + ((char)(38)).ToString() + ((char)(33)).ToString()).ToString();
                    case  74:   return (((char)(36)).ToString() + ((char)(37)).ToString() + ((char)(34)).ToString()).ToString();
                    case  73:   return (((char)(34)).ToString() + ((char)(40)).ToString() + ((char)(33)).ToString()).ToString();
                    case  72:   return (((char)(34)).ToString() + ((char)(37)).ToString() + ((char)(36)).ToString()).ToString();
                    case  71:   return (((char)(33)).ToString() + ((char)(40)).ToString() + ((char)(34)).ToString()).ToString();
                    case  70:   return (((char)(33)).ToString() + ((char)(38)).ToString() + ((char)(36)).ToString()).ToString();
                    case  69:   return (((char)(36)).ToString() + ((char)(34)).ToString() + ((char)(37)).ToString()).ToString();
                    case  68:   return (((char)(36)).ToString() + ((char)(33)).ToString() + ((char)(38)).ToString()).ToString();
                    case  67:   return (((char)(34)).ToString() + ((char)(36)).ToString() + ((char)(37)).ToString()).ToString();
                    case  66:   return (((char)(34)).ToString() + ((char)(33)).ToString() + ((char)(40)).ToString()).ToString();
                    case  65:   return (((char)(33)).ToString() + ((char)(36)).ToString() + ((char)(38)).ToString()).ToString();
                    case  64:   return (((char)(33)).ToString() + ((char)(34)).ToString() + ((char)(40)).ToString()).ToString();
                    case  63:   return (((char)(47)).ToString() + ((char)(33)).ToString() + ((char)(33)).ToString()).ToString();
                    case  62:   return (((char)(38)).ToString() + ((char)(36)).ToString() + ((char)(33)).ToString()).ToString();
                    case  61:   return (((char)(41)).ToString() + ((char)(45)).ToString() + ((char)(33)).ToString()).ToString();
                    case  60:   return (((char)(43)).ToString() + ((char)(37)).ToString() + ((char)(33)).ToString()).ToString();
                    case  59:   return (((char)(41)).ToString() + ((char)(39)).ToString() + ((char)(33)).ToString()).ToString();
                    case  58:   return (((char)(41)).ToString() + ((char)(37)).ToString() + ((char)(35)).ToString()).ToString();
                    case  57:   return (((char)(43)).ToString() + ((char)(33)).ToString() + ((char)(37)).ToString()).ToString();
                    case  56:   return (((char)(41)).ToString() + ((char)(35)).ToString() + ((char)(37)).ToString()).ToString();
                    case  55:   return (((char)(41)).ToString() + ((char)(33)).ToString() + ((char)(39)).ToString()).ToString();
                    case  54:   return (((char)(37)).ToString() + ((char)(41)).ToString() + ((char)(41)).ToString()).ToString();
                    case  53:   return (((char)(37)).ToString() + ((char)(43)).ToString() + ((char)(33)).ToString()).ToString();
                    case  52:   return (((char)(37)).ToString() + ((char)(41)).ToString() + ((char)(35)).ToString()).ToString();
                    case  51:   return (((char)(39)).ToString() + ((char)(33)).ToString() + ((char)(41)).ToString()).ToString();
                    case  50:   return (((char)(37)).ToString() + ((char)(35)).ToString() + ((char)(41)).ToString()).ToString();
                    case  49:   return (((char)(41)).ToString() + ((char)(41)).ToString() + ((char)(37)).ToString()).ToString();
                    case  48:   return (((char)(35)).ToString() + ((char)(41)).ToString() + ((char)(37)).ToString()).ToString();
                    case  47:   return (((char)(33)).ToString() + ((char)(43)).ToString() + ((char)(37)).ToString()).ToString();
                    case  46:   return (((char)(33)).ToString() + ((char)(41)).ToString() + ((char)(39)).ToString()).ToString();
                    case  45:   return (((char)(35)).ToString() + ((char)(37)).ToString() + ((char)(41)).ToString()).ToString();
                    case  44:   return (((char)(33)).ToString() + ((char)(39)).ToString() + ((char)(41)).ToString()).ToString();
                    case  43:   return (((char)(33)).ToString() + ((char)(37)).ToString() + ((char)(43)).ToString()).ToString();
                    case  42:   return (((char)(39)).ToString() + ((char)(35)).ToString() + ((char)(33)).ToString()).ToString();
                    case  41:   return (((char)(39)).ToString() + ((char)(33)).ToString() + ((char)(35)).ToString()).ToString();
                    case  40:   return (((char)(37)).ToString() + ((char)(35)).ToString() + ((char)(35)).ToString()).ToString();
                    case  39:   return (((char)(35)).ToString() + ((char)(39)).ToString() + ((char)(33)).ToString()).ToString();
                    case  38:   return (((char)(35)).ToString() + ((char)(37)).ToString() + ((char)(35)).ToString()).ToString();
                    case  37:   return (((char)(33)).ToString() + ((char)(39)).ToString() + ((char)(35)).ToString()).ToString();
                    case  36:   return (((char)(35)).ToString() + ((char)(35)).ToString() + ((char)(37)).ToString()).ToString();
                    case  35:   return (((char)(35)).ToString() + ((char)(33)).ToString() + ((char)(39)).ToString()).ToString();
                    case  34:   return (((char)(33)).ToString() + ((char)(35)).ToString() + ((char)(39)).ToString()).ToString();
                    case  33:   return (((char)(39)).ToString() + ((char)(37)).ToString() + ((char)(37)).ToString()).ToString();
                    case  32:   return (((char)(37)).ToString() + ((char)(39)).ToString() + ((char)(37)).ToString()).ToString();
                    case  31:   return (((char)(37)).ToString() + ((char)(37)).ToString() + ((char)(39)).ToString()).ToString();
                    case  30:   return (((char)(42)).ToString() + ((char)(38)).ToString() + ((char)(33)).ToString()).ToString();
                    case  29:   return (((char)(42)).ToString() + ((char)(37)).ToString() + ((char)(34)).ToString()).ToString();
                    case  28:   return (((char)(41)).ToString() + ((char)(38)).ToString() + ((char)(34)).ToString()).ToString();
                    case  27:   return (((char)(42)).ToString() + ((char)(34)).ToString() + ((char)(37)).ToString()).ToString();
                    case  26:   return (((char)(42)).ToString() + ((char)(33)).ToString() + ((char)(38)).ToString()).ToString();
                    case  25:   return (((char)(41)).ToString() + ((char)(34)).ToString() + ((char)(38)).ToString()).ToString();
                    case  24:   return (((char)(41)).ToString() + ((char)(37)).ToString() + ((char)(41)).ToString()).ToString();
                    case  23:   return (((char)(38)).ToString() + ((char)(41)).ToString() + ((char)(34)).ToString()).ToString();
                    case  22:   return (((char)(37)).ToString() + ((char)(42)).ToString() + ((char)(34)).ToString()).ToString();
                    case  21:   return (((char)(38)).ToString() + ((char)(34)).ToString() + ((char)(41)).ToString()).ToString();
                    case  20:   return (((char)(38)).ToString() + ((char)(33)).ToString() + ((char)(42)).ToString()).ToString();
                    case  19:   return (((char)(38)).ToString() + ((char)(42)).ToString() + ((char)(33)).ToString()).ToString();
                    case  18:   return (((char)(34)).ToString() + ((char)(42)).ToString() + ((char)(37)).ToString()).ToString();
                    case  17:   return (((char)(34)).ToString() + ((char)(41)).ToString() + ((char)(38)).ToString()).ToString();
                    case  16:   return (((char)(33)).ToString() + ((char)(42)).ToString() + ((char)(38)).ToString()).ToString();
                    case  15:   return (((char)(34)).ToString() + ((char)(38)).ToString() + ((char)(41)).ToString()).ToString();
                    case  14:   return (((char)(34)).ToString() + ((char)(37)).ToString() + ((char)(42)).ToString()).ToString();
                    case  13:   return (((char)(33)).ToString() + ((char)(38)).ToString() + ((char)(42)).ToString()).ToString();
                    case  12:   return (((char)(39)).ToString() + ((char)(34)).ToString() + ((char)(34)).ToString()).ToString();
                    case  11:   return (((char)(38)).ToString() + ((char)(35)).ToString() + ((char)(34)).ToString()).ToString();
                    case  10:   return (((char)(38)).ToString() + ((char)(34)).ToString() + ((char)(35)).ToString()).ToString();
                    case   9:   return (((char)(35)).ToString() + ((char)(38)).ToString() + ((char)(34)).ToString()).ToString();
                    case   8:   return (((char)(34)).ToString() + ((char)(39)).ToString() + ((char)(34)).ToString()).ToString();
                    case   7:   return (((char)(34)).ToString() + ((char)(38)).ToString() + ((char)(35)).ToString()).ToString();
                    case   6:   return (((char)(35)).ToString() + ((char)(34)).ToString() + ((char)(38)).ToString()).ToString();
                    case   5:   return (((char)(34)).ToString() + ((char)(35)).ToString() + ((char)(38)).ToString()).ToString();
                    case   4:   return (((char)(34)).ToString() + ((char)(34)).ToString() + ((char)(39)).ToString()).ToString();
                    case   3:   return (((char)(38)).ToString() + ((char)(38)).ToString() + ((char)(37)).ToString()).ToString();
                    case   2:   return (((char)(38)).ToString() + ((char)(37)).ToString() + ((char)(38)).ToString()).ToString();
                    case   1:   return (((char)(37)).ToString() + ((char)(38)).ToString() + ((char)(38)).ToString()).ToString();
                }
                return "";
            }

        
            protected string optimizeCodesets(string _stringIn, int _stringLength, bool _fnc1)
            {
                string PatternDigits = "^:d*>";

                int         codeset = CodesetA;
                int         cset = 1;
                bool        shift = false;      // Is shift in effect
                int         fcn4 = 1;
                bool        pfcn4 = false;      // Permanent fcn4 shift is set
                string      rtnstr="";             // return string
                int         nchr;
                int         ncodec;
                bool        cont;
                int         i = 1;
                int         j;
                int         k = 0;
                int         charValue;
                int         nextCodeset;
                bool        isdigital;

                // check to see if control codes are already imbedding in input string
                
                charValue = _stringIn.Substring(0,1).ToCharArray()[0];
                if (charValue == EscChar)
                    return _stringIn;

                isdigital = string.Equals(PatternDigits, _stringIn);

                // Check code if it's digit string.
                if (isdigital && _stringLength > 1)
                {
                    rtnstr = StartCOptimizer;
                    codeset = CodesetC;      // set the codeset to c
                }
                else
                {
                    // Check the start of the string to see if it starts with numbers
                    ncodec = this.charInCodesetCount(_stringIn,_stringLength,0,CodesetC);

                    // if string starts with more than 4 number it will be Codeset c
                    if (ncodec >= 4)
                    {
                        // If ncodec is odd then the 1st character should be in Codeset B then go to Code c
                        if (ncodec % 2 > 0)
                        {
                            rtnstr = StartBOptimizer;
                            codeset = CodesetB;    // set the codeset to b
                        }
                        else
                        {
                            // if string starts with even number of numerics start with Code C.
                            rtnstr = StartCOptimizer;
                            codeset = CodesetC;    // set the codeset to c
                        }
                    }
                    else
                    {
                        // if string doesn't start with 4 numbers
                        cont = true;
                        i = 0;
                        // Find if the first character is Codeset A or B.
                        while (cont)
                        {
                            charValue = _stringIn.Substring(i,1).ToCharArray()[0];
                            if ((charValue >= 0) && (charValue <= 31) || charValue > 127) //0 - 31 is unique to a
                            {
                                cont = false;
                                rtnstr = StartAOptimizer;
                                codeset = CodesetA;    // set the codeset to a
                            }
                            else
                            {
                                if ((charValue >= 96) && (charValue <= 127)) //96-127 is unique to b
                                {
                                    cont = false;
                                    rtnstr = StartBOptimizer;
                                    codeset = CodesetB;    // set the codeset to b
                                }
                            }

                            // Default to Codeset B if all characters are in both A & b
                            if (cont && (_stringLength <= i))
                            {
                                cont = false;
                                rtnstr = StartBOptimizer;
                                codeset = CodesetB;    // set the codeset to b
                            }
                            i++;

                        }
                    }
                }

                // add support for fnc1 adding it right after the start code
                if (_fnc1)
                    rtnstr += F1_ENCODE;

                // convert the message
                cont = true;    // whether or not to continue
                i = 0;       // index into input string

                while (cont)
                {
                    j = this.charInCodesetCount(_stringIn,_stringLength,i,codeset);
                    if (j == 0)
                    {
                        rtnstr += (char)(127);
                        i++;
                    }
                    else
                    {
                        if ( isdigital && _stringLength > 1)
                        {
                            if (_stringLength % 2 == 0)
                            {
                                rtnstr += _stringIn;
                            }
                            else
                            {
                                rtnstr += _stringIn.Substring(0, _stringLength - 1) + Shift2BOptimizer + _stringIn.Substring(_stringLength, 1);
                            }
                        }
                        else
                        {
                            for (k = j-1; k>=0; k--)
                            {
                                if (_stringIn.Substring(i,1).ToCharArray()[0] == EscChar)
                                {
                                    rtnstr += _stringIn.Substring(i, 2);
                                    i+=2;
                                    k--;
                                }
                                else
                                {
                                    rtnstr += _stringIn.Substring(i, 1);
                                    i++;
                                }
                            }
                        }
                    }

                    if (i > _stringLength-1 || (isdigital && _stringLength > 1))  // end of string
                        return rtnstr;

                    // process code change
                    switch (codeset)
                    {
                        case CodesetA: // if currently Codeset a
                            charValue = int.Parse(_stringIn.Substring(i-1,1));
                            if ((charValue >= 48) && (charValue <= 57))
                            {
                                nchr = this.charInCodesetCount(_stringIn,_stringLength,i,CodesetC);
                                if (nchr >= 4)
                                {
                                    if (nchr % 2 > 0)
                                    {
                                        rtnstr += _stringIn.Substring(i,1);
                                        i++;
                                    }
                                    rtnstr += Shift2COptimizer;
                                    codeset = CodesetC;
                                }
                                else
                                {
                                    if (nchr > 0)
                                    {
                                        rtnstr += _stringIn.Substring(i, nchr);
                                        i += nchr;
                                    }
                                    else
                                    {
                                        rtnstr += (char)(127);
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                if ((i <= _stringLength) && (this.nextABChange(_stringIn,_stringLength,i) == NextB) && (this.nextABChange(_stringIn,_stringLength,i+1) == NextA))
                                {
                                    if (charValue == 172)
                                        rtnstr += (char)(EscChar) + (char)(98) + (char)(95);
                                    else
                                        rtnstr += (char)(EscChar) + (char)(98) + _stringIn.Substring(i, 1);
                                    i++;
                                }
                                else
                                {
                                    if (this.nextABChange(_stringIn,_stringLength-1,i) == NextB)
                                    {
                                        rtnstr += Shift2BOptimizer;
                                        codeset = CodesetB;
                                    }
                                }
                            }
                            break;
                        case CodesetB: // if currently Codeset b
                            charValue = _stringIn.Substring(i,1).ToCharArray()[0];
                            if ((charValue >= 48) && (charValue <= 57))
                            {
                                nchr = this.charInCodesetCount(_stringIn,_stringLength,i,CodesetC);
                                if (nchr >= 4)
                                {
                                    if (nchr % 2 > 0)
                                    {
                                        rtnstr += _stringIn.Substring(i, 1);
                                        i++;
                                    }
                                    rtnstr += Shift2COptimizer;
                                    codeset = CodesetC;
                                }
                                else
                                {
                                    if (nchr > 0)
                                    {
                                        rtnstr += _stringIn.Substring(i, nchr);
                                        i += nchr;
                                    }
                                    else
                                    {
                                        rtnstr += (char)(127);
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                if ((i <= _stringLength) &&(this.nextABChange(_stringIn,_stringLength,i) == NextA) && (this.nextABChange(_stringIn,_stringLength,i+1) == NextB))
                                {
                                    if (charValue == 172)
                                        rtnstr += (char)(EscChar) + (char)(98) + (char)(200);
                                    else
                                        rtnstr += (char)(EscChar) + (char)(98) + _stringIn.Substring(i, 1);
                                    i++;
                                }
                                else
                                {
                                    if (this.nextABChange(_stringIn,_stringLength,i) == NextA)
                                    {
                                        rtnstr += Shift2AOptimizer;
                                        codeset = CodesetA;
                                    }
                                }
                            }
                            break;
                    
                        case CodesetC: // if currently Codeset c
                            nextCodeset = this.nextABChange(_stringIn,_stringLength,i);
                            switch(nextCodeset)
                            {
                                case NextA:
                                    rtnstr += Shift2AOptimizer;
                                    codeset = CodesetA;
                                    break;
                                case NextB:
                                    rtnstr += Shift2BOptimizer;
                                    codeset = CodesetB;
                                    break;
                                default:
                                    break;
                            }
                            break;
                    }
                }

                return rtnstr;
            }


            protected int nextABChange(string _str,int _strlength, int _startPos)
            {
                int     endrtn = 0;
                int     charValue;
                int     pos;
                ;
                for (pos = _startPos-1; pos <= _strlength; pos++)
                {
                    charValue = int.Parse(_str.Substring(pos,1));
                    if ((charValue >= 96) && (charValue <= 127))
                        return NextB;
                    if ((charValue >=  1) && (charValue <=  31))
                        return NextA;
                    if (charValue ==  172)
                        return NextA;
                    if ((charValue >= 32) && (charValue <=  95))
                        endrtn = NextB;
                }
                return endrtn;
            }


            // Counts how many characters starting at _startPos that are in the _codeset
            protected int charInCodesetCount(string _str, int _strLength, int _startPos, int _codeset)
            {
                bool    cont = true;
                int     retnum = 0;
                int     nchr;
                int     i;
                int     charThisPos;
                bool    anyF1 = false;
                int     digitsBeforeF1=0;

                switch (_codeset)
                {
                    case CodesetA:

                        for (i = _startPos; i <= _strLength; i++)
                        {
                            charThisPos = int.Parse(_str.Substring(i,1));
                            if (((charThisPos >= 0) && (charThisPos <= 47)) || ((charThisPos >= 58) && (charThisPos <= 95)) || (charThisPos > 127))
                            {
                                retnum++;
                            }
                            else
                            {
                                if ((charThisPos >= 48) && (charThisPos <= 57))
                                {
                                    nchr = this.followingCharsBetween(_str,_strLength,i,48,57);
                                    if (nchr >= 4)
                                        i = _strLength + 1; // break the loop
                                    else
                                    {
                                        retnum += nchr;
                                        i += nchr - 1;
                                    }
                                }
                                else
                                    i = _strLength + 1; // break the loop
                            }
                        }
                        break;
                    case CodesetB:
                        for (i = _startPos; i <= _strLength; i++)
                        {
                            charThisPos = int.Parse(_str.Substring(i,1));
                            if (((charThisPos >= 32) && (charThisPos <= 47)) || ((charThisPos >= 58) && (charThisPos <= 127)))
                            {
                                retnum++;
                            }
                            else
                            {
                                if ((charThisPos >= 48) && (charThisPos <= 57))
                                {
                                    nchr = this.followingCharsBetween(_str,_strLength,i,48,57);
                                    if (nchr >= 4)
                                    {
                                        i = _strLength + 1; // break the loop to force a switch to codesetc
                                        if (nchr % 2 == 1)
                                            retnum += 1;
                                    }
                                    else
                                    {
                                        retnum += nchr;
                                        i += nchr - 1;
                                    }
                                }
                                else
                                    i = _strLength + 1; // break the loop
                            }
                        }
                        break;
                    case CodesetC:
                        for (i = _startPos; i <= _strLength; i++)
                        {
                            charThisPos = _str.Substring(i,1).ToCharArray()[0];
                            if ((charThisPos >= 48) && (charThisPos <= 57))
                            {
                                retnum++;
                            }
                            else
                            if ((retnum % 2 == 0) && (_str.Substring(i,2) == F1_ENCODE)) // equal number of digits followed by FNC1
                            {
                                if (!anyF1)
                                {
                                    digitsBeforeF1 = retnum;
                                }
                                retnum += 2;
                                i++;
                                anyF1 = true;
                            }
                            else
                            {
                                i = _strLength + 1; // break the loop
                            }
                        }
                        if (anyF1 && (retnum % 2 != 0))
                        {
                            retnum = digitsBeforeF1;
                        }
                        break;
                }
                return retnum;
            }

            protected int followingCharsBetween(string _str, int _strLength, int _startPos, int _minChar, int _maxChar)
            {
                int retnum = 0;
                int i;
                int charValue;
                ;
                for (i = _startPos; i <= _strLength; i++)
                {
                    charValue =  int.Parse(_str.Substring(i,1));
                    if ((charValue < _minChar) || (charValue > _maxChar))
                        return retnum;
                    retnum++;
                }
                return retnum;
            }
    }    
}
