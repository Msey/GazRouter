<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >GDU_H:F,30.08.2017 10:00:00,10758,
NORTGAZ1_H:F,30.08.2017 10:00:00,496,
NORTGAZ1_H:P,30.08.2017 10:00:00,57.2,
NORTGAZ1_H:T,30.08.2017 10:00:00,5,
NORTGAZ1_H:TTR,30.08.2017 10:00:00,-30,
NORTGAZ2_H:F,30.08.2017 10:00:00,484,
NORTGAZ2_H:P,30.08.2017 10:00:00,60.3,
NORTGAZ2_H:T,30.08.2017 10:00:00,5,
NORTGAZ2_H:TTR,30.08.2017 10:00:00,-30,
ROSPAN_N_H:F,30.08.2017 10:00:00,453,
ROSPAN_N_H:P,30.08.2017 10:00:00,64.6,
ROSPAN_N_H:T,30.08.2017 10:00:00,15,
ROSPAN_N_H:TTR,30.08.2017 10:00:00,-18,
ROSPAN_V_H:F,30.08.2017 10:00:00,224,
ROSPAN_V_H:P,30.08.2017 10:00:00,58.7,
ROSPAN_V_H:T,30.08.2017 10:00:00,20,
ROSPAN_V_H:TTR,30.08.2017 10:00:00,-23,
GAZD_H:F,30.08.2017 10:00:00,55.98333,
GAZD_H:P,30.08.2017 10:00:00,55.25055,
GAZD_H:T,30.08.2017 10:00:00,15.42056,
GAZD_H:TTR,30.08.2017 10:00:00,-20.03674,
GTS_REVERS_H:F,30.08.2017 10:00:00,739,


    

  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <xsl:param name='timeTemplate' >[^,]*,(\d\d).(\d\d).(\d\d\d\d)\s(\d\d):(\d\d):(\d\d),(.)*,</xsl:param>
  <!-- метка времени сеанса-->
  <xsl:param name="timestamp_var">
    <xsl:analyze-string select="$rows[1]" flags="x" regex='{$timeTemplate}'>
      <xsl:matching-substring>
        <xsl:variable name='dt' select="concat(regex-group(3),'-',regex-group(2),'-',regex-group(1), 'T', regex-group(4), ':00:00')"/>
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
  <xsl:param name='regexTemplate' >([A-Za-z0-9_:]{1,30}),(\d{2}.\d{2}.\d{4}\s\d{2}:\d{2}:\d{2}),([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d),</xsl:param>

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
                    <xsl:value-of select="replace(normalize-space(regex-group(3)),',','.' )"/>
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