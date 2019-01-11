<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >1: : 2017083014: 
2: : 2017083014: 
3: 130.000: 2017083014: 
4: 10.600: 2017083014: 
5: 14.200: 2017083014: 
6: -35.000: 2017083014: 
7: 1010.000: 2017083014: 
8: 1: 2017083014: 
9: : 2017083014: 
10: : 2017083014: 
11: : 2017083014: 
12: : 2017083014: 
13: : 2017083014: 
14: : 2017083014: 
15: : 2017083014: 
16: : 2017083014: 
17: 0: 2017083014: 
18: : 2017083014: 
19: : 2017083014: 
20: : 2017083014: 
21: 10.600: 2017083014: 
22: 15.700: 2017083014: 
23: -40.000: 2017083014: 
24: 130.000: 2017083014: 
25: 2: 2017083014: 
26: : 2017083014: 
27: : 2017083014: 
28: : 2017083014: 
29: : 2017083014: 
30: : 2017083014: 
31: : 2017083014: 
32: 0: 2017083014: 
33: : 2017083014: 
34: : 2017083014: 
35: : 2017083014: 
36: : 2017083014: 
37: : 2017083014: 
38: : 2017083014: 
39: 0: 2017083014: 
40: : 2017083014: 
41: : 2017083014: 
42: 10.800: 2017083014: 
43: 17.000: 2017083014: 
44: -38.000: 2017083014: 
45: 200.000: 2017083014: 
46: 2: 2017083014: 
47: : 2017083014: 
48: : 2017083014: 
49: : 2017083014: 
50: : 2017083014: 
51: : 2017083014: 
52: : 2017083014: 
53: 0: 2017083014: 
54: : 2017083014: 
55: : 2017083014: 
56: 10.800: 2017083014: 
57: 18.000: 2017083014: 
58: -15.000: 2017083014: 
59: 154.000: 2017083014: 
60: 1: 2017083014: 
61: : 2017083014: 
62: : 2017083014: 
63: 11.400: 2017083014: 
64: 16.500: 2017083014: 
65: -19.500: 2017083014: 
66: 211.000: 2017083014: 
67: 2: 2017083014: 
68: : 2017083014: 
69: : 2017083014: 
70: : 2017083014: 
71: : 2017083014: 
72: : 2017083014: 
73: : 2017083014: 
74: 15.800: 2017083014: 
75: 62.200: 2017083014: 
76: 15.900: 2017083014: 
77: 11.400: 2017083014: 
78: -15.000: 2017083014: 
79: 1950.000: 2017083014: 
80: 62.900: 2017083014: 
81: 13.900: 2017083014: 
82: 15.000: 2017083014: 
83: 9.500: 2017083014: 
84: 25.600: 2017083014: 
85: : 2017083014: 
86: 46.396: 2017083014: 
87: : 2017083014: 
88: 0.400: 2017083014: 
89: -32.300: 2017083014: 
90: 187.000: 2017083014: 
91: 17.022: 2017083014: 
92: 12.067: 2017083014: 
93: -31.749: 2017083014: 
94: 24632.290: 2017083014: 
95: 61.863: 2017083014: 
96: 11.058: 2017083014: 
97: -21.042: 2017083014: 
98: 27018.000: 2017083014: 
99: 62.650: 2017083014: 
100: 12.017: 2017083014: 
101: -15.375: 2017083014: 
102: 46907.000: 2017083014: 
104: 23.000: 2017083014: 
105: 34.333: 2017083014: 
106: 108.000: 2017083014: 
108: 6.080: 2017083014: 
110: 110.000: 2017083014: 
111: 61.998: 2017083014: 
112: 7.100: 2017083014: 
113: -27.000: 2017083014: 


  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <xsl:param name='timeTemplate' >[^:]*:[^:]*\s{0,1}(\d\d\d\d)(\d\d)(\d\d)(\d\d):</xsl:param>
  <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:analyze-string select="$rows[1]" flags="x" regex='{$timeTemplate}'>
      <xsl:matching-substring>
        <xsl:variable name='dt' select="concat(regex-group(1),'-',regex-group(2),'-',regex-group(3), 'T', regex-group(4), ':00:00')"/>
        <xsl:choose>
          <xsl:when test="string($dt) castable as xs:dateTime">
            <xsl:value-of select="$dt" />
          </xsl:when>
          <xsl:otherwise>
            <xsl:value-of select="$dt" />
          </xsl:otherwise>
        </xsl:choose>
        <xsl:message>
          dt+: <xsl:value-of select='.' />
        </xsl:message>
      </xsl:matching-substring>
      <xsl:non-matching-substring>
        <xsl:message>
          dt-: <xsl:value-of select='.' />
        </xsl:message>
      </xsl:non-matching-substring>
    </xsl:analyze-string>
  </xsl:param>

  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений    -->
  <xsl:param name='regexTemplate' >(\d{1,5}):\s([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d):\s(\d{10}):</xsl:param>

  <xsl:template match='/'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp"/>
        </TimeStamp>
        <PeriodType>
          <xsl:value-of select="$periodType"/>
        </PeriodType>
      </HeaderSection>
      <DataSection>
        <ExtItems>
          <xsl:for-each select='$rows'>
            <xsl:analyze-string select='.' flags='x' regex='{$regexTemplate}'>
              <xsl:matching-substring>
                <ExtItem>
                  <ExtKey>
                    <xsl:value-of select="normalize-space(regex-group(1))"/>
                  </ExtKey>
                  <Value>
                    <xsl:value-of select="replace(normalize-space(regex-group(2)),',','.' )"/>
                  </Value>
                </ExtItem>
                <xsl:message>
                  EQUAL: <xsl:value-of select='.' />
                </xsl:message>
              </xsl:matching-substring>
              <xsl:non-matching-substring>
                <xsl:message>
                  -: <xsl:value-of select='.' />
                </xsl:message>
              </xsl:non-matching-substring>
            </xsl:analyze-string>
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>