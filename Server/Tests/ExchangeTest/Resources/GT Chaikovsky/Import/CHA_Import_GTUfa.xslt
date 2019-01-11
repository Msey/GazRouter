<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  <!-- разделитель строк входного файла-->
  <xsl:param name='rowDelimiter'>\r?\n</xsl:param>

  <!-- преобразуемая информация, во время выполнения подменяется значениями из входных файлов от определенного ДООО  -->
  <xsl:param name='inputContent' as='xs:string'   >
20011;1;56.4;
20011;2;70;
20011;3;23;
20011;4;37;
20011;12;42;
20012;1;56.5;
20012;3;24;
20012;4;37;
20012;12;44;
20013;2;69.9;
20013;3;26;
20013;4;38;
20014;1;57.3;
20014;2;70.8;
20014;12;47;
20015;1;56.8;
20015;2;70.3;
20015;4;38;
20015;12;46;
20000;6;1*1;
20002;6;2*2;
20011;6;2*2;
20010;6;2*1;
20012;6;2*1;
20015;6;3*1;
20000;1;58;
20000;2;68.4;
20000;3;17;
20000;4;31;
20000;12;34;
20001;1;57;
20001;2;69.1;
20014;3;17;
20001;3;17;
20001;4;31;
20001;12;36;
20002;1;57.3;
20002;2;69;
20002;3;17;
20002;4;31;
20002;12;35;
20010;1;56.5;
20010;2;70.5;
20010;3;22;
20010;12;43;
20010;4;37;
20000;13;1245;
20010;5;13;
20013;13;234;
20013;5;15;
20001;13;14567;
20002;5;1467;
20002;13;235;
20011;5;1356;
20011;13;247;
20012;5;34;
20014;5;1257;
20014;13;348;
20015;5;236;
20015;13;14578;
20012;2;70.6;
20013;1;57.6;
20013;12;45;
20014;4;38;
20015;3;27;
20000;5;3;
20010;13;245;
20001;5;238;
20012;13;15;
20001;6;1*2 1*1;
20013;6;1*1 1*1;
20014;6;2*2 0*0;
end
  </xsl:param>

  <!-- разбивка на массив строк -->
  <xsl:variable name='rows' as='xs:string*' select="tokenize($inputContent, $rowDelimiter)"/>

  <!-- метка времени сеанса-->
  <xsl:param name="timestamp"/>

  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <!-- регулярное выражение, по которому парсится строка на группы ключей и значений -->
  <xsl:param name='regexTemplate' >(\d{1,7});(\d{1,4});([+-]{0,1}\d{0,10}\.{0,1}\d{0,10}|\d\*\d|\d\*\d\s\d\*\d);$</xsl:param>
 <!-- регулярное выражение, по которой парсится строка со схемой работы -->
   <xsl:param name='regexScheme' >(\d)\*(\d)\s{0,1}(\d{0,1})\*{0,1}(\d{0,1})</xsl:param>
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
            <xsl:variable name='pos' select="position()"/>
            <xsl:analyze-string select='.' flags='x' regex='{$regexTemplate}'>
              <xsl:matching-substring>
                  <xsl:variable name='matchStr' as='xs:string' select="regex-group(1)"/>              
                
                  <xsl:variable name='extKey' as='xs:string' select="concat(normalize-space(regex-group(1)),';',normalize-space(regex-group(2))  )"/>
                 <xsl:variable name='value1' as='xs:string' select="normalize-space( regex-group(3) )"/>
                  <!-- Проверка на тип свойств от УФЫ, если 5 (номера агрегатов в работе) или 13(номера агрегатов в резерве),то считается их количество 
                  6 - схема работы-->
                  <xsl:choose>
                    <xsl:when test="normalize-space(regex-group(2)) = '5'">
                      <ExtItem>
                        <ExtKey><xsl:value-of select="$extKey"/></ExtKey>
                        <Value><xsl:value-of select="string-length( $value1  )"/></Value>
                      </ExtItem>
                    </xsl:when>
                    <xsl:when test="normalize-space(regex-group(2)) = '13'">
                      <ExtItem>
                        <ExtKey><xsl:value-of select="$extKey"/></ExtKey>
                        <Value><xsl:value-of select="string-length( $value1 )"/></Value>
                      </ExtItem>
                    </xsl:when>
                     <!-- из схемы формируется 3 значения: количество ступеней, кол-во групп, схема \ 6 - схема работы
                     groupCount
                     compressionStageCount
                     compressorShopPattern
                       -->
                     <xsl:when test="normalize-space(regex-group(2)) = '6'">
                        <xsl:analyze-string select='.' flags='x' regex='{$regexScheme}'>
                              <xsl:matching-substring>
                                 <xsl:variable name='Ngr' as='xs:string' select="normalize-space(regex-group(1))"/>
                                 <xsl:variable name='Nstup' as='xs:string' select="normalize-space(regex-group(2))"/>
                                <ExtItem>
                                  <ExtKey><xsl:value-of select="concat( $extKey,'_groupCount')"/></ExtKey>
                                  <Value><xsl:value-of select="$Ngr"/></Value>
                                </ExtItem>
                                <ExtItem>
                                  <ExtKey><xsl:value-of select="concat( $extKey,'_compressionStageCount')"/></ExtKey>
                                  <Value><xsl:value-of select="$Nstup"/></Value>
                                </ExtItem>
                                <ExtItem>
                                  <ExtKey><xsl:value-of select="concat( $extKey,'_compressorShopPattern')"/></ExtKey>
                                  <Value><xsl:value-of select="concat( $Ngr,'X' ,$Nstup )"/></Value>
                                </ExtItem>
                                    <!--<Ngr><xsl:value-of select="normalize-space(regex-group(1))"/></Ngr>
                                        <Nstup><xsl:value-of select="normalize-space(regex-group(2))"/></Nstup>
                                        <ExtItem>
                                        <ExtKey><xsl:value-of select="concat( $extKey,'_compressorShopPattern')"/></ExtKey>
                                  <Value><xsl:value-of select="concat( $Ngr,'X' ,$Nstup )"/></Value>
                                  <Str><xsl:value-of select="."/></Str>
                                </ExtItem>
                                        <xsl:message>+: <xsl:value-of select='.' /> </xsl:message>
                                    -->
                              </xsl:matching-substring>
                              <!--<xsl:non-matching-substring>
                                    <xsl:message>-: <xsl:value-of select='.' /></xsl:message>
                              </xsl:non-matching-substring>-->
                        </xsl:analyze-string>
                    </xsl:when>
                    <xsl:otherwise>
                       <ExtItem>
                          <ExtKey><xsl:value-of select="$extKey"/></ExtKey>
                          <Value><xsl:value-of select="$value1"/></Value>
                          <!--<raw><xsl:value-of select="."/></raw>-->
                       </ExtItem>
                    </xsl:otherwise>
                  </xsl:choose>
                  <!--<xsl:message>+: <xsl:value-of select='.' /> </xsl:message>-->
              </xsl:matching-substring>
              <!--<xsl:non-matching-substring>
                <xsl:message>-: <xsl:value-of select='.' /></xsl:message>
              </xsl:non-matching-substring>-->
            </xsl:analyze-string>
          </xsl:for-each>
        </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
</xsl:stylesheet>

