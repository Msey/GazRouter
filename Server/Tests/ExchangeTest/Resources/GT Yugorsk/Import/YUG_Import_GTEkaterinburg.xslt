﻿<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >    6:	34.800:	2017090112
    7:	17.000:	2017090112
    8:	0.200:	2017090112

  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <xsl:param name='timeTemplate' >[^:]*:[^:]*\s{0,1}(\d\d\d\d)(\d\d)(\d\d)(\d\d).*</xsl:param>
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
  <xsl:param name='regexTemplate' >(\d{1,5}):.([+-]{0,1}\d{0,10}.{0,1}\d{0,10}|\dx\d):.(\d{10})</xsl:param>

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