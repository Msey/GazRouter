<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="CP866" />

  <xsl:decimal-format NaN="" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:variable name="timestamp">
      <xsl:value-of select="format-dateTime(TimeStamp, '[D01].[M01].[Y0001]')" />
    </xsl:variable>
    <xsl:value-of select="$timestamp" />
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <!-- compressorShops -->
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:sort select="ExtKey"/>

        <xsl:value-of select="ExtKey" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorShopPattern']/Value" />
        <xsl:text>, </xsl:text>
        <!-- может заменить -->
        <xsl:value-of select="format-number(Properties/Property[SysName='fuelGasConsumption']/Value, '0.000000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(../../Valves/Valve[Item/CompShopId=current()/Item/Id]/Properties/Property[SysName='pressureInlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(../../Valves/Valve[Item/CompShopId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureCooling']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(../../CompressorStations/CompressorStation[Item/Id=current()/Item/ParentId]/Properties/Property[SysName='temperatureAir']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorUnitsInReserve']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorUnitsUnderRepair']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='coolingUnitsInUse']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='fuelGasConsumption']/Value, '0.000000')" />
        <xsl:text>, ,</xsl:text>
        <xsl:value-of select="Item/EntityType" />
        <xsl:text>:</xsl:text>
        <xsl:value-of select="Item/Id" />
        <xsl:text> </xsl:text>
        <xsl:value-of select="Item/ShortPath" />
        <xsl:text>&#13;&#10;</xsl:text>

        <xsl:for-each select="../../CompUnits/CompUnit[Item/ParentId=current()/Item/Id]">
          <xsl:sort select="ExtKey"/>

          <xsl:value-of select="ExtKey" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='compressorUnitState']/Value" />
          <xsl:text>, </xsl:text>
          <!-- может заменить -->
          <xsl:value-of select="format-number(Properties/Property[SysName='fuelGasConsumption']/Value, '0.0000000')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureSuperchargerInlet']/Value, '0.000')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureSuperchargerOutlet']/Value, '0.000')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureSuperchargerInlet']/Value, '0.000')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureSuperchargerOutlet']/Value, '0.000')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='rpmSupercharger']/Value" />
          <xsl:text>, ,, </xsl:text>
          <xsl:value-of select="Item/EntityType" />
          <xsl:text>:</xsl:text>
          <xsl:value-of select="Item/Id" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Item/ShortPath" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:for-each>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
      <xsl:for-each select="DistributingStations/DistributingStation">
        <xsl:sort select="ExtKey"/>

        <xsl:value-of select="ExtKey" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
        <xsl:text>, ,, </xsl:text>
        <xsl:value-of select="Item/EntityType" />
        <xsl:text>:</xsl:text>
        <xsl:value-of select="Item/Id" />
        <xsl:text> </xsl:text>
        <xsl:value-of select="Item/ShortPath" />
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
    <xsl:if test="count(MeasureLines/MeasureLine) &gt; 0">
      <xsl:for-each select="MeasureLines/MeasureLine">
        <xsl:sort select="ExtKey"/>

        <xsl:value-of select="ExtKey" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(../../MeasurePoints/MeasurePoint[Item/MeasLineId=current()/Item/Id]/Properties/Property[SysName='densityAbsolute']/Value, '0.0')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="format-number(../../MeasurePoints/MeasurePoint[Item/MeasLineId=current()/Item/Id]/Properties/Property[SysName='combustionHeatLow']/Value, '0.0')" />
        <xsl:text>, ,, </xsl:text>
        <xsl:value-of select="Item/EntityType" />
        <xsl:text>:</xsl:text>
        <xsl:value-of select="Item/Id" />
        <xsl:text> </xsl:text>
        <xsl:value-of select="Item/ShortPath" />
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>

    <xsl:if test="count(ReducingStations/ReducingStation) &gt; 0">
      <xsl:for-each select="ReducingStations/ReducingStation">
        <xsl:sort select="ExtKey"/>

        <xsl:variable name="prg1">
          <xsl:value-of select="substring-before(ExtKey, '::')" />
        </xsl:variable>
        <xsl:variable name="prg2">
          <xsl:value-of select="substring-before(substring-after(ExtKey, '::'), '|')" />
        </xsl:variable>
        <xsl:variable name="prg3">
          <xsl:value-of select="substring-after(ExtKey, '|')" />
        </xsl:variable>

        <!-- Короткий -->
        <xsl:if test="string-length($prg1)&gt; 0">
          <xsl:value-of select="$prg1" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
          <xsl:text>, , ,</xsl:text>
          <xsl:value-of select="Item/EntityType" />
          <xsl:text>:</xsl:text>
          <xsl:value-of select="Item/Id" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Item/ShortPath" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
        <!-- Длинный -->
        <xsl:if test="string-length($prg2)&gt; 0">
          <xsl:value-of select="$prg2" />
          <xsl:text>, 000, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0')" />
          <xsl:text>, ,</xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.0')" />
          <xsl:text>, ,,</xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0.0')" />
          <xsl:text>,,,,,,,,,</xsl:text>
          <xsl:value-of select="Item/EntityType" />
          <xsl:text>:</xsl:text>
          <xsl:value-of select="Item/Id" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Item/ShortPath" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
        <!-- Длинный -->
        <xsl:if test="string-length($prg3)&gt; 0">
          <xsl:value-of select="$prg3" />
          <xsl:text>, 000, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0')" />
          <xsl:text>, ,</xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureInlet']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='pressureOutlet']/Value, '0.0')" />
          <xsl:text>, ,,</xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureInlet']/Value, '0.0')" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="format-number(Properties/Property[SysName='temperatureOutlet']/Value, '0.0')" />
          <xsl:text>,,,,,,,,,</xsl:text>
          <xsl:value-of select="Item/EntityType" />
          <xsl:text>:</xsl:text>
          <xsl:value-of select="Item/Id" />
          <xsl:text> </xsl:text>
          <xsl:value-of select="Item/ShortPath" />
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>
      </xsl:for-each>
    </xsl:if>

  </xsl:template>
</xsl:stylesheet>