using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using static System.Math;

namespace GazRouter.Flobus.Utilites
{
    public class InternalGeometryParser
    {
        private readonly PathGeometry _geometry;
        private string _buffer;
        private int _length;
        private int _index;
        private char _token;
        private PathFigure _figure;
        private Point _lastPoint;

        public InternalGeometryParser(PathGeometry geometry)
        {
            _geometry = geometry;
        }

        public void Parse(string data, int startIndex)
        {
            _buffer = data;
            _length = data.Length;
            _index = startIndex;

            var flag = true;
            while (ReadToken())
            {
                var token = _token;
                if (flag)
                {
                    if ((token != 'M') && (token != 'm'))
                    {
                        InvalidFormat();
                    }
                    flag = false;
                }
                switch (token)
                {
                    case 'Q':
                    case 'q':
                    {
                        throw new NotSupportedException();
                    }
                    case 'M':
                    case 'm':
                    {
                        _lastPoint = ReadPoint(token, false);
                        BeginFigure(_lastPoint);
                        char command;
                        for (command = 'M'; IsNumber(true); command = 'L')
                        {
                            _lastPoint = ReadPoint(command, false);
                            LineTo(_lastPoint);
                        }
                        continue;
                    }
                    case 'L':
                    case 'l':
                    {
                        EnsureFigure();
                        do
                        {
                            _lastPoint = ReadPoint(token, false);
                            LineTo(_lastPoint);
                        }
                        while (IsNumber(true));
                        continue;
                    }
                    case 'Z':
                    case 'z':
                    {
                        FinishFigure(true);
                        continue;
                    }
                    case 'S'
                        :
                    case 's':
                    case 'V':
                    case 'v':

                    case 'A':
                    case 'a':
                    case 'C':
                    case 'c':

                        throw new NotImplementedException(token.ToString());
                }
                throw new NotSupportedException();
            }
            FinishFigure(false);
        }

        private void LineTo(Point point)
        {
            var segment = new LineSegment {Point = point};
            _figure.Segments.Add(segment);
        }

        private void BeginFigure(Point startPoint)
        {
            FinishFigure(false);
            EnsureFigure();
            _figure.StartPoint = startPoint;
            _figure.IsFilled = true;
        }

        private void FinishFigure(bool figureExplicityClosed)
        {
            if (_figure == null)
            {
                return;
            }

            if (figureExplicityClosed)
            {
                _figure.IsClosed = true;
            }

            _geometry.Figures.Add(_figure);
            _figure = null;
        }

        private Point ReadPoint(char command, bool allowComa)
        {
            var x = ReadDouble(allowComa);
            var y = ReadDouble(true);
            if (command >= 'a')
            {
                x += _lastPoint.X;
                y += _lastPoint.Y;
            }
            return new Point(x, y);
        }

        private double ReadDouble(bool allowComa)
        {
            double num;
            if (!IsNumber(allowComa))
            {
                InvalidFormat();
            }
            var flag = true;
            var index = _index;
            if (More() && ((_buffer[_index] == '-') || (_buffer[_index] == '+')))
            {
                _index++;
            }
            if (More() && (_buffer[_index] == 'I'))
            {
                _index = Min(_index + 8, _length);
                flag = false;
            }
            else if (More() && (_buffer[_index] == 'N'))
            {
                _index = Min(_index + 3, _length);
                flag = false;
            }
            else
            {
                SkipDigits(false);
                if (More() && (_buffer[_index] == '.'))
                {
                    flag = false;
                    _index++;
                    SkipDigits(false);
                }
                if (More() && ((_buffer[index] == 'E') || (_buffer[_index] == 'e')))
                {
                    flag = false;
                    _index++;
                    SkipDigits(true);
                }
            }

            if (flag && (_index <= index + 8))
            {
                var num3 = 1;
                if (_buffer[index] == '+')
                {
                    index++;
                }
                else if (_buffer[index] == '-')
                {
                    index++;
                    num3 = -1;
                }
                var num4 = 0;
                while (index < _index)
                {
                    num4 = num4*10 + (_buffer[index] - '0');
                    index++;
                }
                return num4*num3;
            }

            var str = _buffer.Substring(index, _index - index);
            try
            {
                num = Convert.ToDouble(str, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new FormatException();
            }
            return num;
        }

        private void SkipDigits(bool signAllowed)
        {
            if (signAllowed && More() && ((_buffer[_index] == '-') || (_buffer[_index] == '+')))
            {
                _index++;
            }
            while (More() && (_buffer[_index] >= '0') && (_buffer[_index] <= '9'))
            {
                _index++;
            }
        }

        private bool IsNumber(bool allowComma)
        {
            var flag = SkipWhitespace(allowComma);
            if (More())
            {
                _token = _buffer[_index];
                if ((_token == '.') || (_token == '-') || (_token == '+') || ((_token >= '0') && (_token <= '9')) ||
                    (_token == 'I') || (_token == 'N'))
                {
                    return true;
                }
            }
            if (flag)
            {
                InvalidFormat();
            }

            return false;
        }

        private void EnsureFigure()
        {
            if (_figure == null)
            {
                _figure = new PathFigure {Segments = new PathSegmentCollection()};
            }
        }

        private bool ReadToken()
        {
            SkipWhitespace(false);
            if (More())
            {
                _token = _buffer[_index++];
                return true;
            }
            return false;
        }

        private bool SkipWhitespace(bool allowComma)
        {
            var flag = false;
            while (More())
            {
                var c = _buffer[_index];
                switch (c)
                {
                    case '\t':
                    case '\n':
                    case '\r':
                    case ' ':
                        break;
                    case ',':
                        if (!allowComma)
                        {
                            InvalidFormat();
                        }
                        flag = true;
                        allowComma = false;
                        break;
                    default:
                        if ((c > ' ') && c <= 'z')
                        {
                            return flag;
                        }

                        if (!char.IsWhiteSpace(c))
                        {
                            return flag;
                        }
                        break;
                }
                _index++;
            }
            return false;
        }

        private void InvalidFormat()
        {
            throw new FormatException();
        }

        private bool More()
        {
            return _index < _length;
        }
    }
}