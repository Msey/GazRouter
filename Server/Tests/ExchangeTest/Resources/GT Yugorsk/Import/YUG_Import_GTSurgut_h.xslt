<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'>1: 3054.700: 2017083114
    2: 2900.100: 2017083114
    3: 3230.500: 2017083114
    4: 1160.000: 2017083114
    5: 2775.000: 2017083114
    6: 0.000: 2017083114
    7: 191.000: 2017083114
    8: 844.000: 2017083114
    9: 2301.800: 2017083114
    10: 2275.800: 2017083114
    11: 4774.800: 2017083114
    12: 1812.000: 2017083114
    14: 0: 2017083114
    15: 918.000: 2017083114
    16: 738.000: 2017083114
    17: 349.000: 2017083114
    18: 45.4: 2017083114
    19: 45.4: 2017083114
    20: 45.4: 2017083114
    21: 52.2: 2017083114
    22: 64.9: 2017083114
    23: 58.4: 2017083114
    24: 55.9: 2017083114
    25: 70.8: 2017083114
    26: 70.8: 2017083114
    27: 58.3: 2017083114
    28: 57.6: 2017083114
    29: 7.1: 2017083114
    30: 4.3: 2017083114
    31: 5.8: 2017083114
    32: 10: 2017083114
    33: 14: 2017083114
    34: 12: 2017083114
    35: 7: 2017083114
    36: 17.3: 2017083114
    37: 14.9: 2017083114
    38: 8.5: 2017083114
    39: -20: 2017083114
    40: -22.4: 2017083114
    41: -20.8: 2017083114
    42: -15: 2017083114
    43: -16: 2017083114
    44: -28: 2017083114
    45: -17: 2017083114
    46: 19: 2017083114
    47: -16.2: 2017083114
    48: -19.5: 2017083114
    49: -24.3: 2017083114
    50: -25.2: 2017083114
    51: 856.000: 2017083114
    52: 79.000: 2017083114
    53:0: 2017083114
    54:0: 2017083114


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
  <xsl:param name='regexTemplate' >(\d{1,5}):\s([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d):\s(\d{10})</xsl:param>

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