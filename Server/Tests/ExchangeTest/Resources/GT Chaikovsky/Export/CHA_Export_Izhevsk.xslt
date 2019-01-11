<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text" />
  <xsl:decimal-format NaN="" />
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExcahangeMessage/HeaderSection">
    <xsl:text>Газпром межрегионгаз Ижевск&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:variable name="Date">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[D01][M01][Y0001]')" />
  </xsl:variable>
  <xsl:variable name="Seans">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[H01]')" />
  </xsl:variable>

  <xsl:template match="ExchangeMessage/DataSection">
    <!-- выход ГРС -->
    <xsl:if test="count(Outputs/Output)">
      <xsl:for-each select="Outputs/Output">
        <xsl:value-of select="ExtKey" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$Seans" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$Date" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value*1000, '0')" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.00')" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0')" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="Item/ShortPath" />
        <xsl:text>;</xsl:text>

        <xsl:variable name="FullPath">
          <xsl:value-of select="Item/Path" />
        </xsl:variable>
        <xsl:variable name='substr1' as='xs:string*' select="tokenize($FullPath, '/')"/>
        <xsl:value-of select="$substr1[2]" />
        
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>


