<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >Бутузов Е.А.         16.08.12 11:13:53 v.11.12.7
    LPU,10,Tvozd,+23.0
    CEX,27,Scheme,0x0
    CEX,27,Pvh,52.7
    CEX,27,Pvih,0.0
    CEX,27,Tvh,+0.0
    CEX,27,Tvih,+0.0
    CEX,28,Scheme,0x0
    CEX,28,Pvh,52.7
    CEX,28,Pvih,0.0
    CEX,28,Tvh,+0.0
    CEX,28,Tvih,+0.0
    CEX,29,Scheme,0x0
    CEX,29,Pvh,51.7
    CEX,29,Pvih,0.0
    CEX,29,Tvh,+0.0
    CEX,29,Tvih,+0.0
    CEX,83,Scheme,1x1
    CEX,83,Pvh,51.7
    CEX,83,Pvih,94.0
    CEX,83,Tvh,+28.7
    CEX,83,Tvih,+30.0
    GIS,49,Pvh,0.0
    GIS,49,Tvh,+0.0
    GIS,49,Tr,+0.0
    GIS,49,Q,+0.000
    GIS,50,Pvh,0.0
    GIS,50,Tvh,+0.0
    GIS,50,Tr,+0.0
    GIS,50,Q,+0.000
    GIS,51,Pvh,0.0
    GIS,51,Tvh,+0.0
    GIS,51,Tr,+0.0
    GIS,51,Q,+0.000
    GIS,52,Pvh,0.0
    GIS,52,Tvh,+0.0
    GIS,52,Tr,+0.0
    GIS,52,Q,+0.000
    GIS,59,Pvh,0.0
    GIS,59,Tvh,+0.0
    GIS,59,Tr,+0.0
    GIS,59,Q,+0.000
    GIS,53,Pvh,53.7
    GIS,53,Tvh,+28.7
    GIS,53,Tr,+0.0
    GIS,53,Q,+259.000
    GIS,55,Pvh,91.5
    GIS,55,Tvh,+30.0
    GIS,55,Tr,+0.0
    GIS,55,Q,+259.000
    GIS,56,Pvh,91.5
    GIS,56,Tvh,+30.0
    GIS,56,Tr,+0.0
    GIS,56,Q,+226.000
    GIS,57,Pvh,91.5
    GIS,57,Tvh,+30.0
    GIS,57,Tr,+0.0
    GIS,57,Q,+18.000
    GIS,58,Pvh,91.2
    GIS,58,Tvh,+30.0
    GIS,58,Tr,+0.0
    GIS,58,Q,+15.000
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name="timestamp"/>
  

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