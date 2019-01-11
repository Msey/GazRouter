<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   > 03:	740:	2017083106
    04:	3,38:	2017083106
    05:	-2:	2017083106
    06:	-27,8:	2017083106
    07:	180:	2017083106
    08:	3,91:	2017083106
    09:	8,5:	2017083106
    11:	240:	2017083106
    12:	3,38:	2017083106
    13:	-2:	2017083106
    14:	-23:	2017083106
    15:	NaN:	2017083106
    16:	NaN:	2017083106
    17:	NaN:	2017083106
    18:	NaN:	2017083106
    19:	1015:	2017083106
    20:	3,4:	2017083106
    21:	-2:	2017083106
    22:	-26,5:	2017083106
    23:	690:	2017083106
    24:	4,26:	2017083106
    25:	8,2:	2017083106
    26:	0:	2017083106
    27:	NaN:	2017083106
    28:	NaN:	2017083106
    29:	NaN:	2017083106
    30:	NaN:	2017083106
    31:	221:	2017083106
    32:	3,33:	2017083106
    33:	-2:	2017083106
    34:	-28,4:	2017083106
    35:	NaN:	2017083106
    36:	NaN:	2017083106
    37:	NaN:	2017083106
    38:	NaN:	2017083106
    40:	1672:	2017083106
    41:	3,53:	2017083106
    42:	-3,2:	2017083106
    43:	-30:	2017083106
    45:	9,69:	2017083106
    46:	24,7:	2017083106
    47:	NaN:	2017083106
    49:	5,34:	2017083106
    50:	24,9:	2017083106
    51:	NaN:	2017083106
    52:	180:	2017083106
    53:	3,91:	2017083106
    54:	8,5:	2017083106
    55:	2:	2017083106
    56:	2600:	2017083106
    57:	5,25:	2017083106
    58:	-1,5:	2017083106
    59:	-23:	2017083106
    60:	533:	2017083106
    61:	5,83:	2017083106
    62:	10,2:	2017083106
    63:	NaN:	2017083106
    64:	521:	2017083106
    65:	5,89:	2017083106
    66:	9,1:	2017083106
    67:	NaN:	2017083106
    69:	4157,12:	2017083106
    70:	4,66:	2017083106
    71:	9,1:	2017083106
    72:	-16,4:	2017083106
    73:	3496,85:	2017083106
    74:	4,73:	2017083106
    75:	8,78:	2017083106
    76:	-24,38:	2017083106
    77:	0:	2017083106
    78:	0:	2017083106
    79:	3,44:	2017083106
    80:	-19,6:	2017083106
    81:	923,94:	2017083106
    82:	4,72:	2017083106
    83:	-0,82:	2017083106
    84:	-36,33:	2017083106
    85:	667,28:	2017083106
    86:	4,84:	2017083106
    87:	-3,06:	2017083106
    88:	-36,3:	2017083106
    89:	10,62:	2017083106
    90:	5,1:	2017083106
    91:	-9,9:	2017083106
    92:	-35,42:	2017083106



  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <xsl:param name='timeTemplate' >[^:]*:[^:]*\s{0,1}(\d\d\d\d)(\d\d)(\d\d)(\d\d).*</xsl:param>
  <!-- метка времени сеанса-->
  <xsl:param name="timestamp_var">
    <xsl:analyze-string select="$rows[1]" flags="x" regex='{$timeTemplate}'>
      <xsl:matching-substring>
        <xsl:variable name='dt' select="concat(regex-group(1),'-',regex-group(2),'-',regex-group(3), 'T', regex-group(4), ':00:00')"/>
        <xsl:choose>
          <xsl:when test="string($dt) castable as xs:dateTime">
            <xsl:value-of select="format-dateTime(xs:dateTime($dt) + xs:dayTimeDuration('PT2H'), '[Y0001]-[M01]-[D01]T[H01]:[m01]:[s01]')" />
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
  <xsl:param name='regexTemplate' >(\d{1,5}):.([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d):.(\d{10})</xsl:param>

  <xsl:template match='/'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <xsl:value-of select="$timestamp_var"/>
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