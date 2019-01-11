<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >Чибирев С.В.         16.08.12 10:47:57 v.10.3.29
LPU,9,Tvozd,+28.0
CEX,24,Scheme,
CEX,24,Pvh,
CEX,24,Pvih,
CEX,24,Tvh,
CEX,24,Tvih,
CEX,25,Scheme,1x1
CEX,25,Pvh,43.7
CEX,25,Pvih,55.0
CEX,25,Tvh,+25.0
CEX,25,Tvih,+29.0
CEX,37,Scheme,1x1
CEX,37,Pvh,54.5
CEX,37,Pvih,75.0
CEX,37,Tvh,+29.0
CEX,37,Tvih,+35.0
CEX,26,Scheme,2x1
CEX,26,Pvh,74.4
CEX,26,Pvih,150.0
CEX,26,Tvh,+33.0
CEX,26,Tvih,+40.0
GIS,27,Pvh,
GIS,27,Tvh,
GIS,27,Tr,
GIS,27,Q,+0.000
GIS,28,Pvh,
GIS,28,Tvh,
GIS,28,Tr,
GIS,28,Q,+0.000
GIS,29,Pvh,
GIS,29,Tvh,
GIS,29,Tr,
GIS,29,Q,+0.000
GIS,39,Pvh,
GIS,39,Tvh,
GIS,39,Tr,
GIS,39,Q,+0.000
GIS,31,Pvh,43.7
GIS,31,Tvh,+17.8
GIS,31,Tr,-13.7
GIS,31,Q,+243.000
GIS,33,Pvh,149.5
GIS,33,Tvh,+31.0
GIS,33,Tr,-13.7
GIS,33,Q,+243.000
GIS,34,Pvh,149.0
GIS,34,Tvh,+32.0
GIS,34,Tr,-13.7
GIS,34,Q,+164.000
GIS,35,Pvh,150.0
GIS,35,Tvh,+29.0
GIS,35,Tr,-13.7
GIS,35,Q,+79.000
GIS,30,Pvh,44.0
GIS,30,Tvh,+27.0
GIS,30,Tr,+22.4
GIS,30,Q,+6.100
KRN,912,Open,00
KRN,913,Open,-1
GIS,86,Tsgor,8092
GIS,86,D_Plotn,15.08.12
GIS,86,D_Tr,15.08.12
GIS,86,D_Vlaga,15.08.12
GIS,86,D_MKS,
GIS,86,D_MKH,
GIS,86,D_Tsgor,24.07.12
GIS,86,D_Pplast,10.08.12
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
  <xsl:param name='regexTemplate' >(\D{3,5},\d{1,4},\D{1,8}) , ([+-]{0,1}\d{1,10}[\.x]{0,1}\d{0,10} $   )</xsl:param> 
 <!--<xsl:param name='regexTemplate' >(\D{3,5},\d{1,4},\D{1,8}) , ( \d\d\.\d\d\.\d\d )</xsl:param>-->
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
                  DATA+: <xsl:value-of select='.' />
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