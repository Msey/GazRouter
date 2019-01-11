<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- разделитель колонок входного файла-->
  <xsl:param name='columnDelimiter'>:</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >
    4001;1100;2x2
    4001;2;51.4
    4001;4;66.3
    4001;22;24.0
    4001;23;50.0
    4001;24;39.0
    4001;27;21.0
    4001;25;14.0
    4003;1100;4x1
    4003;2;50.9
    4003;4;66.0
    4003;22;22.0
    4003;23;47.0
    4003;24;36.0
    4005;1100;0x0
    4005;2;53.0
    4005;4;64.6
    4005;22;24.0
    4005;23;0.0
    4005;24;36.0
    4002;4;65.4
    4002;59;3574.000
    4004;4;64.6
    4004;59;2751.000
    4006;4;64.6
    4006;59;0.000
    4007;4;33.4
    4007;23;15.0
    4007;25;15.0
    4007;26;-11.7
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name="timestamp"/>

  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений -->
  <xsl:param name='regexTemplate' >^\s*([^;]*)\s*;\s*([^;]*)\s*;\s*([^;]*)\s*$</xsl:param>

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
                    <xsl:variable name="extKey">
                      <xsl:value-of select="concat(normalize-space(regex-group(1)),';',normalize-space(regex-group(2))  )"/>
                    </xsl:variable>
                    <xsl:value-of select="$extKey"/>
                  </ExtKey>
                  <Value>
                    <xsl:value-of select="normalize-space(regex-group(3))"/>
                  </Value>
                </ExtItem>
              </xsl:matching-substring>
            </xsl:analyze-string>
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>