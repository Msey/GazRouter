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
    <xsl:text>Газпром трансгаз Саратов&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <!-- по всем цехам-->
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:for-each select="Properties/Property">
          <!-- схема работы-->
          <xsl:choose>           
            <xsl:when test="SysName = 'compressorShopPattern'">
                <xsl:text>CEX;</xsl:text>
                <xsl:value-of select="../../ExtKey" />
                <xsl:text>;Scheme;</xsl:text>
                <xsl:value-of select="Value" />
                <xsl:text>;#Схема работы, </xsl:text>
                <xsl:value-of select="../../Item/ShortPath" />
                <xsl:text>&#10;</xsl:text>
            </xsl:when>            
          </xsl:choose>
          <!-- давление на входе -->
          <xsl:choose>
            <xsl:when test="SysName = 'pressureInlet'">
              <xsl:text>CEX;</xsl:text>
              <xsl:value-of select="../../ExtKey" />
              <xsl:text>;Pvh;</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>;#Pвх, </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
          </xsl:choose>
          <!-- давление на выходе -->
          <xsl:choose>
            <xsl:when test="SysName = 'pressureOutlet'">
              <xsl:text>CEX;</xsl:text>
              <xsl:value-of select="../../ExtKey" />
              <xsl:text>;Pvih;</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>;#Pвых, </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
          </xsl:choose>
          <!-- температура на входе -->
          <xsl:choose>
            <xsl:when test="SysName = 'temperatureInlet'">
              <xsl:text>CEX;</xsl:text>
              <xsl:value-of select="../../ExtKey" />
              <xsl:text>;Tvh;</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>;#Твх, </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
          </xsl:choose>
          <!-- температура на выходе -->
          <xsl:choose>
            <xsl:when test="SysName = 'temperatureOutlet'">
              <xsl:text>CEX;</xsl:text>
              <xsl:value-of select="../../ExtKey" />
              <xsl:text>;Tvih;</xsl:text>
              <xsl:choose>
                <xsl:when test="string(Value) castable as xs:double">
                  <xsl:value-of select="format-number(Value, '0.0')" />
                </xsl:when>
                <xsl:otherwise>
                  <xsl:value-of select="Value" />
                </xsl:otherwise>
              </xsl:choose>
              <xsl:text>;#Твых, </xsl:text>
              <xsl:value-of select="../../Item/ShortPath" />
              <xsl:text>&#10;</xsl:text>
            </xsl:when>
          </xsl:choose>
          
        </xsl:for-each>   
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
