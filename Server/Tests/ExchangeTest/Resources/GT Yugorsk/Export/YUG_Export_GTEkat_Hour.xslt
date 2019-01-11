<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:variable name="timestamp">
    <xsl:value-of select="format-dateTime(xs:dateTime(ExchangeMessage/HeaderSection/TimeStamp) - xs:dayTimeDuration('PT2H'), '[Y0001][M01][D01][H01]')" />
  </xsl:variable>

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


  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:variable name="v_13" select="number(MeasureLines/MeasureLine/Properties/Property[ExtKey='32014']/Value)"/>
    <xsl:variable name="v_46" select="sum(DistributingStations/DistributingStation/Properties/Property[SysName='flow']/Value)"/>
    <!--  and ExtKey != '32018'-->

    <xsl:if test="count(*/*/Properties/Property) &gt; 0">
      <xsl:for-each select="*/*/Properties/Property">
        <xsl:sort select="ExtKey"/>

        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="ExtKey" />
          <xsl:text>:	  </xsl:text>

          <xsl:choose>
            <xsl:when test="ExtKey = '32014'">
              <!-- f_TTG_Hparams(@Data,2))-->
              <xsl:value-of select="func:FormatValString(string($v_13 - $v_46), '0.0' )"/>
            </xsl:when>

            <xsl:when test="ExtKey = '32016'">
              <!-- f_TTG_Hparams(@Data,46))-->
              <xsl:value-of select="func:FormatValString(string($v_46), '0.0' )"/>
            </xsl:when>

            <xsl:otherwise>
              <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
            </xsl:otherwise>
          </xsl:choose>

          <xsl:text>	:</xsl:text>
          <xsl:value-of select="$timestamp" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>