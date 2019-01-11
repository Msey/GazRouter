<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >GDU_D:F,29.08.2017 13:00:00,298227,
    GDU_GRES_D:F,29.08.2017 13:00:00,2065,
    GDU_GMRGS_D:F,29.08.2017 13:00:00,50,
    ROSPAN_N_D:F,29.08.2017 13:00:00,10839,
    ROSPAN_V_D:F,29.08.2017 13:00:00,5414,
    NORTGAZ_D:F,29.08.2017 13:00:00,23402,
    GTS_REVERS_D:F,29.08.2017 13:00:00,0,
    GAZD_D:F,29.08.2017 13:00:00,998,




  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <xsl:param name='timeTemplate' >[^,]*,(\d\d).(\d\d).(\d\d\d\d)\s(\d\d):(\d\d):(\d\d),(.)*,</xsl:param>
  <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:analyze-string select="$rows[1]" flags="x" regex='{$timeTemplate}'>
      <xsl:matching-substring>
        <xsl:variable name='dt' select="concat(regex-group(3),'-',regex-group(2),'-',regex-group(1), 'T', regex-group(4), ':', regex-group(5), ':', regex-group(6))"/>
        <xsl:choose>
          <xsl:when test="string($dt) castable as xs:dateTime">
            <xsl:value-of select="format-dateTime(xs:dateTime($dt) - xs:dayTimeDuration('P1D'), '[Y0001]-[M01]-[D01]T[H01]:[m01]:[s01]')" />
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
  <xsl:param name='periodType'>Day</xsl:param>

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений    -->
  <xsl:param name='regexTemplate' >([A-Za-z0-9_:]{1,30}),(\d{2}.\d{2}.\d{4}\s\d{2}:\d{2}:\d{2}),([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d),</xsl:param>

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