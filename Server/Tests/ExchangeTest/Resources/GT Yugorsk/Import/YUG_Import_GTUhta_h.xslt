<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >0:20171011120508
    21:	34,18	:2017101112
    22:	43,45	:2017101112
    23:	7,53	:2017101112
    24:	22,7	:2017101112
    25:	7,88	:2017101112
    31:	56,3	:2017101112
    32:	71,04	:2017101112
    33:	8,17	:2017101112
    34:	29,09	:2017101112
    35:	7,88	:2017101112
    41:	56,25	:2017101112
    42:	71,03	:2017101112
    43:	8,86	:2017101112
    44:	25,14	:2017101112
    45:	7,88	:2017101112
    51:	70,63	:2017101112
    52:	70,63	:2017101112
    53:	9,73	:2017101112
    54:	19,1	:2017101112
    55:	7,88	:2017101112
    61:	36,74	:2017101112
    62:	38,58	:2017101112
    63:	6,28	:2017101112
    64:	2,42	:2017101112
    65:	4,69	:2017101112
    66:	-24,88	:2017101112
    67:	67,94	:2017101112
    71:	60,96	:2017101112
    72:	71,25	:2017101112
    73:	8,41	:2017101112
    74:	22,89	:2017101112
    75:	4,69	:2017101112
    76:	-22,09	:2017101112
    77:	36,08	:2017101112
    81:	61,17	:2017101112
    82:	71,39	:2017101112
    83:	7,84	:2017101112
    84:	22,12	:2017101112
    85:	4,69	:2017101112
    86:	-22,84	:2017101112
    87:	68,18	:2017101112
    91:	61,69	:2017101112
    92:	71,25	:2017101112
    93:	9,09	:2017101112
    94:	11,1	:2017101112
    95:	4,69	:2017101112
    96:	-18,64	:2017101112
    97:	68,33	:2017101112

  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>


  <xsl:param name='timeTemplate' >0:(\d{4})(\d{2})(\d{2})(\d{2})(\d{2})(\d{2})</xsl:param>
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
  <xsl:param name='regexTemplate' >(\d{1,5}):.([+-]{0,1}\d{0,10},{0,1}\d{0,10}|\dx\d).:(\d{10})</xsl:param>

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