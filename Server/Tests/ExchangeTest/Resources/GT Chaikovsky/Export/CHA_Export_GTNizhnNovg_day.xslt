<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text" />
  <xsl:decimal-format NaN="" />
  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:text>Газпром трансгаз Нижний Новгород&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:variable name="Date">
    <xsl:value-of select="format-dateTime(ExchangeMessage/HeaderSection/TimeStamp, '[D01][M01][Y0001]')" />
  </xsl:variable>

  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(MeasureLines/MeasureLine) &gt; 0">
      <!--  ГИС -->
      <xsl:for-each select="MeasureLines/MeasureLine">
        <xsl:value-of select="ExtKey" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="$Date" />
        <xsl:text>;</xsl:text>
        <xsl:value-of select="Properties/Property[SysName='flow']/Value" />
        <xsl:text>;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>