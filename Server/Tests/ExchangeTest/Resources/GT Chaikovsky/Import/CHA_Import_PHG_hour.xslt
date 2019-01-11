<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version='2.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform' xmlns:xs='http://www.w3.org/2001/XMLSchema'>
  
  <xsl:output omit-xml-declaration='no' indent='yes' version='1.0' encoding='utf-8'/>
  <!-- метка времени сеанса-->
  <xsl:param name="timestamp">
    <xsl:variable name='dtStr' select="BusinessMessage/HeaderSection/ReferenceTime/@time"/>
    <xsl:value-of select="substring($dtStr,1,19)" />
  </xsl:param>
  <!-- тип периода сеанса -->
  <xsl:param name='periodType'>Twohours</xsl:param>

  <xsl:template match='BusinessMessage'>
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
            <xsl:for-each select='DataSection'>
              <ExtItem>
                <ExtKey><xsl:value-of select ='Identifier'/></ExtKey>
                <Value><xsl:value-of select ='Value'/></Value>
              </ExtItem>
            </xsl:for-each >
          </ExtItems>
      </DataSection>
    </ExchangeMessage>
  </xsl:template>
 
  
   
</xsl:stylesheet>