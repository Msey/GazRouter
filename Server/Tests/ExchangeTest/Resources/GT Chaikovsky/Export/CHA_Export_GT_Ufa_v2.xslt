<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" 	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <!-- КЦ  -->
    <xsl:for-each select="CompressorShops/CompressorShop">
      <xsl:variable name="compShopKey">
        <xsl:value-of select="ExtKey" />
      </xsl:variable>
      <xsl:for-each select="Properties/Property">
        <xsl:if test="count(ExtKey) != 0">
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
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
      <!--качество газа на КЦ-->
      <xsl:for-each select="../../MeasurePoints/MeasurePoint[Item/CompShopId=current()/Item/Id]/Properties/Property">
        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="ExtKey" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="format-number(Value, '0.#')" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>



    </xsl:for-each>

    <!-- ГИС  -->
    <xsl:for-each select="MeasureLines/MeasureLine">
      <xsl:variable name="measLineKey">
        <!--<xsl:value-of select="substring-after(ExtKey, 'gis_')" /> -->
        <xsl:value-of select="substring-after(ExtKey, 'gis_')" />
      </xsl:variable>
      <xsl:for-each select="Properties/Property">
        <xsl:if test="count(ExtKey) != 0">
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
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
      <!--качество газа на ЗЛ-->
      <xsl:for-each select="../../MeasurePoints/MeasurePoint[Item/MeasLineId=current()/Item/Id]/Properties/Property">
        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="ExtKey" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="format-number(Value, '0.#')" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:for-each>

    <!-- КС  -->
    <xsl:for-each select="CompressorStations/CompressorStation">
      <xsl:variable name="compStationKey">
        <xsl:value-of select="ExtKey" />
      </xsl:variable>
      <xsl:for-each select="Properties/Property">
        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="$compStationKey" />
          <xsl:text>;</xsl:text>
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
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:for-each>



    <!--Номера ГПА в работе (state=1) - 5, резерве (state=2) =13-->
    <xsl:for-each-group select="CompUnits/CompUnit" group-by="Item/ParentId">
      <xsl:variable name="shopExtId">
        <xsl:value-of select="../../CompressorShops/CompressorShop[Item/Id=current-grouping-key()]/ExtKey" />
      </xsl:variable>

      <xsl:variable name="strWork" />
      <xsl:variable name="strReserve" />

      <xsl:value-of select="$shopExtId" />
      <xsl:text>;</xsl:text>
      <xsl:text>5;</xsl:text>
      <xsl:for-each select="current-group()">

        <xsl:choose>
          <xsl:when test="Properties/Property[SysName='compressorUnitState']/Value = 1">
            <xsl:value-of select="Item/CompUnitNum" />
            <!--<xsl:value-of select="Item/ShortPath" />-->
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
      <xsl:text>&#13;&#10;</xsl:text>

      <xsl:value-of select="$shopExtId" />
      <xsl:text>;</xsl:text>
      <xsl:text>13;</xsl:text>
      <xsl:for-each select="current-group()">
        <xsl:choose>
          <xsl:when test="Properties/Property[SysName='compressorUnitState']/Value = 2">
            <xsl:value-of select="Item/CompUnitNum" />
            <!--<xsl:value-of select="Item/ShortPath" />-->
          </xsl:when>
        </xsl:choose>
      </xsl:for-each>
      <xsl:text>&#13;&#10;</xsl:text>
    </xsl:for-each-group>


  </xsl:template>
</xsl:stylesheet>