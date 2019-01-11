<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>

  
  <xsl:template match='ExchangeMessage'>
    <BusinessMessage>
      <HeaderSection>
        <Sender id="ГП ТГ Чайковский"/>
        <Receiver id="ГП ПХГ"/>
        <Generated>
            <xsl:attribute name="at">
              <!--xsl:value-of select ='HeaderSection/GeneratedTime'/-->
              <xsl:value-of select ="format-dateTime(HeaderSection/GeneratedTime, '[Y0001]-[M01]-[D01]T[H01]:[m01]:[s01][Z]')"/>              
            </xsl:attribute>
        </Generated>
        <Comment>Файл данных от ГТ Чайковский</Comment>
        <ReferenceTime>
            <xsl:attribute name="time">
              <!-- должно быть так:  <ReferenceTime time="2017-08-08T10:00:00+03:00"/>-->
              <xsl:value-of select ="format-dateTime(HeaderSection/TimeStamp, '[Y0001]-[M01]-[D01]T10:00:00[Z]')"/>
            </xsl:attribute>
        </ReferenceTime>
        <Scale>PT24H</Scale>
        <Template id="T_CHA.PT24H.UB.V1"/>
        <FullName/>
      </HeaderSection>
            <xsl:for-each select='//Property'>
              <xsl:if test="count(ExtKey) &gt; 0">
                <DataSection>
                  <Identifier  type="G_PHG">
                    <xsl:value-of select ='ExtKey'/>
                  </Identifier>
                  <Value>
                    <xsl:value-of select ='Value'/>
                  </Value>
                </DataSection>
              </xsl:if>
            </xsl:for-each >
    </BusinessMessage>
  </xsl:template>
 
  
   
</xsl:stylesheet>