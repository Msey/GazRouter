<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">

  <xsl:output method="text" />

  <xsl:decimal-format NaN=""  decimal-separator="."/>
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>
  
  <!-- функция -->
  <xsl:function name="func:FormatValString" as="xs:string">
    <xsl:param name="NameVal" as="xs:string"/>
    <xsl:param name="NameFormat" as="xs:string"/>

    <xsl:choose>
      <xsl:when test="$NameVal castable as xs:double">
        <xsl:sequence select="format-number(number($NameVal), $NameFormat)" />
      </xsl:when>
      <xsl:otherwise>
        <xsl:sequence select="$NameVal" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:function>

  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[D01].[M01].[Y0001] 00:00:00')" />
  </xsl:variable>
  
  <xsl:template match="ExchangeMessage/DataSection">

    <xsl:if test="count(*/*/Properties/Property) &gt; 0">
      <xsl:for-each select="*/*/Properties/Property">
        <xsl:sort select="ExtKey"/>

        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="substring-after(ExtKey, ':')" />
          <xsl:text>_D,</xsl:text>
          
          <xsl:value-of select="$timestamp" />          
          <xsl:text>,</xsl:text>
          
          <xsl:choose>
            
            <xsl:when test="substring-after(ExtKey, ':') = 'GTU_S02C01Q1'">          
             
              <xsl:variable name="Q_Val" select ="sum(/ExchangeMessage/DataSection/MeasureLines/MeasureLine[Item/ParentId=current()/../../Item/ParentId]/Properties/Property[SysName='flow']/Value)" />              
              <xsl:value-of select="func:FormatValString(string($Q_Val), '0.0' )"/>
            
            </xsl:when>
            <xsl:otherwise>
            
              <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
      
            </xsl:otherwise>
          </xsl:choose>              
          
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>

