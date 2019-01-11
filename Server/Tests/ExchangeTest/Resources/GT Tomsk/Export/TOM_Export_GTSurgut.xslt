<?xml version="1.0" encoding="utf-8"?>
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
      <xsl:value-of select="format-dateTime(TimeStamp, '[D01].[M01].[Y0001]')" />
    </xsl:variable>
    <xsl:value-of select="$timestamp" />
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">
    <!-- compressorShops -->
    <xsl:if test="count(CompressorShops/CompressorShop) &gt; 0">
      <xsl:for-each select="CompressorShops/CompressorShop">
        <xsl:value-of select="format-number(ExtKey,'0000')" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorShopPattern']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='fuelGasConsumption']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="../../Valves/Valve[Item/CompressorShopId=current()/Item/Id]/Properties/Property[SysName='pressureInlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='pressureInlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='pressureOutlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="../../Valves/Valve[Item/CompressorShopId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='temperatureInlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='temperatureOutlet']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='temperatureCooling']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="../../CompressorStations/CompressorStation[Item/Id=current()/Item/ParentId]/Properties/Property[SysName='temperatureAir']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorUnitsInReserve']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='compressorUnitsUnderRepair']/Value" />
        <xsl:text>, </xsl:text>
        <xsl:value-of select="Properties/Property[SysName='coolingUnitsInUse']/Value" />
        <xsl:text>, ,, </xsl:text>
        <xsl:value-of select="Item/EntityType" />
        <xsl:text>:</xsl:text>
        <xsl:value-of select="Item/Id" />
        <xsl:text> </xsl:text>
        <xsl:value-of select="Item/ShortPath" />
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
    <!-- compressorUnits by compressorShop -->
    <xsl:if test="count(CompUnits/CompUnit) &gt; 0">
      <xsl:for-each-group select="CompUnits/CompUnit" group-by="Item/ParentId">
        <xsl:variable name="K">
          <xsl:value-of select="../../CompressorShops/CompressorShop[Item/Id=current-grouping-key()]/ExtKey" />
        </xsl:variable>
        <xsl:for-each select="current-group()">
          <xsl:value-of select="format-number($K,'0000')" />
          <xsl:text>/</xsl:text>
          <xsl:value-of select="position()" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='compressorUnitState']/Value" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='fuelGasConsumption']/Value" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='pressureSuperchargerInlet']/Value" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='pressureSuperchargerOutlet']/Value" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='temperatureSuperchargerInlet']/Value" />
          <xsl:text>, </xsl:text>
          <xsl:value-of select="Properties/Property[SysName='temperatureSuperchargerOutlet']/Value" />
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
      </xsl:for-each-group>
    </xsl:if>
    
    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
			<xsl:for-each select="DistributingStations/DistributingStation">
				<xsl:value-of select="format-number(ExtKey,'0000')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0')" />
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
				<xsl:value-of select="format-number(ExtKey,'0000')" />
				<xsl:text>, </xsl:text>
				<xsl:value-of select="format-number(Properties/Property[SysName='flow']/Value, '0.0')" />
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
  </xsl:template>
</xsl:stylesheet>