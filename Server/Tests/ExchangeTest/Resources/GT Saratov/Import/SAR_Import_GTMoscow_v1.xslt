<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>
  
  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >4:20120814120000							
1:	0,000	:2012081412					
2:	51,300	:2012081412					
3:	51,300	:2012081412					
4:	15,000	:2012081412					
5:	15,000	:2012081412					
6:	0x1	:2012081412					
7:	51,200	:2012081412					
8:	51,200	:2012081412					
9:	15,000	:2012081412					
10:	15,000	:2012081412					
11:	1	:2012081412					
13:	924,000	:2012081412					
14:	0,000	:2012081412					
19:	38,000	:2012081412					
20:	0,000	:2012081412
  </xsl:param>
  
 <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>
  
 <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:analyze-string select="$rows[1]" flags="x" regex="[^:]*\s*:\s*(\d\d\d\d)(\d\d)(\d\d)(\d\d)(\d\d)(\d\d)\s*">
      <xsl:matching-substring>
        <xsl:value-of select="concat(regex-group(1),'-',regex-group(2),'-',regex-group(3), 'T', regex-group(4), ':', regex-group(5), ':', regex-group(6))"/>
      </xsl:matching-substring>
      <xsl:non-matching-substring>
        <xsl:value-of select="."/>
      </xsl:non-matching-substring>
    </xsl:analyze-string>
  </xsl:param>
  
  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений -->
  <xsl:param name='regexTemplate' >^\s*([^:]*)\s*:\s*([^:]*)\s*:[^:]*$</xsl:param>
  
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
                              </xsl:matching-substring>
                            </xsl:analyze-string>
                          </xsl:for-each>
                </ExtItems>
            </DataSection>
      </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>