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
    <xsl:variable name="timestamp">
      <xsl:value-of select="TimeStamp" />
    </xsl:variable>
    <xsl:text>-1@-1;</xsl:text>
    <xsl:value-of select="format-dateTime($timestamp, '[Y0001].[M01].[D01] [H01]:[m01]')" />
    <xsl:text>&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
      <xsl:for-each select="CompressorShops/CompressorShop">
         <xsl:variable name="compShopKey">
             <xsl:value-of select="ExtKey" />
         </xsl:variable>
         <xsl:for-each select="Properties/Property">
             <xsl:if test="count(ExtKey) != 0">
                      <xsl:value-of select="$compShopKey" />
                      <xsl:text>@</xsl:text>
                      <xsl:value-of select="ExtKey" />
                      <xsl:text>;</xsl:text>
                      <xsl:choose>
                          <xsl:when test="string(Value) castable as xs:double">
                              <xsl:value-of select="format-number(Value, '0.#')" />
                          </xsl:when>
                          <xsl:otherwise>
                              <xsl:value-of select="Value" />
                          </xsl:otherwise>
                      </xsl:choose>
                      <xsl:text>&#10;</xsl:text>
               </xsl:if>
        </xsl:for-each>
        <!--качество газа на КЦ-->
        <xsl:for-each select="../../MeasurePoints/MeasurePoint[Item/CompShopId=current()/Item/Id]/Properties/Property">
              <xsl:if test="count(ExtKey) != 0">
                      <xsl:value-of select="$compShopKey" />
                      <xsl:text>@</xsl:text>
                      <xsl:value-of select="ExtKey" />
                      <xsl:text>;</xsl:text>
                      <xsl:value-of select="format-number(Value, '0.#')" />
                      <xsl:text>&#10;</xsl:text>
              </xsl:if>
      </xsl:for-each>
    </xsl:for-each>

  </xsl:template>
</xsl:stylesheet>