<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >Новичихин Н.И.       16.08.12 11:12:34 v.10.3.29
    LPU,8,Tvozd,+25.8
    CEX,23,Scheme,2x1
    CEX,23,Pvh,52.8
    CEX,23,Pvih,101.8
    CEX,23,Tvh,+27.6
    CEX,23,Tvih,+97.7
    GIS,73,Pvh,0.0
    GIS,73,Tvh,+0.0
    GIS,73,Tr,+0.0
    GIS,73,Q,+0.0
    GIS,77,Pvh,0.0
    GIS,77,Tvh,+0.0
    GIS,77,Tr,+0.0
    GIS,77,Q,+0.0
    GIS,74,Pvh,52.8
    GIS,74,Tvh,+27.6
    GIS,74,Tr,+0.0
    GIS,74,Q,+413.0
    GIS,76,Pvh,52.8
    GIS,76,Tvh,-27.6
    GIS,76,Tr,+0.0
    GIS,76,Q,+413.0
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:analyze-string select="$rows[1]" flags="x" regex="(\d\d)\.(\d\d)\.(\d\d)\s(\d\d):(\d\d):(\d\d)">
      <xsl:matching-substring>
          <xsl:variable name='dt' select="concat(regex-group(1),'-',regex-group(2),'-','20',regex-group(3), 'T', regex-group(4), ':', regex-group(5), ':', regex-group(6))"/>
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

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений -->
  <xsl:param name='regexTemplate' >(\D{3,5},\d{1,2},\D{1,6}),([+-]{0,1}\d{0,10}\.\d{0,10}|\dx\d)</xsl:param>

  <xsl:template match='/'>
    <ExchangeMessage>
      <HeaderSection>
        <TimeStamp>
          <!--<xsl:value-of select="$timestamp"/>-->
        </TimeStamp>
        <PeriodType>
          <xsl:value-of select="$periodType"/>
        </PeriodType>
        <GeneratedTime>
          <xsl:value-of select="$timestamp"/>
        </GeneratedTime>
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
                        <xsl:value-of select="replace(normalize-space(regex-group(2)),'\+','' )"/> 
                    </Value>
                  </ExtItem>
                <xsl:message>
                  data+: <xsl:value-of select='.' />
                </xsl:message>
              </xsl:matching-substring>
              <xsl:non-matching-substring>
                <xsl:message>
                  data-: <xsl:value-of select='.' />
                </xsl:message>
              </xsl:non-matching-substring>
            </xsl:analyze-string>
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>