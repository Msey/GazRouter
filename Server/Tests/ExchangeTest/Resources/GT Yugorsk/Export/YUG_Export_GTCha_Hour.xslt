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

  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:text>Тюментрансгаз</xsl:text>
    <xsl:text>&#13;&#10;</xsl:text>


  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">

    <xsl:variable name="timestamp">
      <xsl:value-of select="format-dateTime(xs:dateTime(../HeaderSection/TimeStamp) - xs:dayTimeDuration('PT2H'), '[D01][M01][Y0001];[H01]')" />
    </xsl:variable>

    <xsl:if test="count(*/*/Properties/Property) &gt; 0">
      <xsl:for-each select="*/*/Properties/Property">
        <xsl:sort select="ExtKey"/>

        <xsl:if test="count(ExtKey) != 0">
          <xsl:value-of select="substring-before(replace(ExtKey, ' ', ''), ':')" />
          <xsl:text>;</xsl:text>
          <xsl:value-of select="$timestamp" />
          <xsl:text>;</xsl:text>

          <!-- Обработка тип 1-->
          <xsl:if test="substring-after(ExtKey, ':') = '1'">

            <xsl:variable name="pressureInlet" select="Value"/>
            <xsl:variable name="pressureOutlet" select="../Property[SysName='pressureOutlet']/Value"/>
            <xsl:variable name="temperatureInlet" select="../Property[SysName='temperatureInlet']/Value"/>            
            <xsl:variable name="temperatureCooling" select="../Property[SysName='temperatureCooling']/Value"/>
            
            <xsl:variable name="pressureIn20" select="../../../../Valves/Valve[Item/CompShopId=current()/../../Item/Id]/Properties/Property[SysName='pressureInlet']/Value"/>
            <xsl:variable name="pressureOut20" select="../../../../Valves/Valve[Item/CompShopId=current()/../../Item/Id]/Properties/Property[SysName='pressureOutlet']/Value"/>
            <xsl:variable name="temperatureIn20" select="../../../../Valves/Valve[Item/CompShopId=current()/../../Item/Id]/Properties/Property[SysName='temperatureInlet']/Value"/>            
            <xsl:variable name="temperatureOut20" select="../../../../Valves/Valve[Item/CompShopId=current()/../../Item/Id]/Properties/Property[SysName='temperatureOutlet']/Value"/>

            <!-- Формирование выходных параметров-->
            <xsl:choose>
              <!--        
        IF  @P_VX = 0 AND  @P_VYX = 0
        BEGIN
           SET @P_1 = @P_VX20
           SET @P_2 = @P_VYX20         
           SET @T_1 = @DPBX
           SET @T_2 = @DPBYX         
        END -->
              <xsl:when test="$pressureInlet = '0.0' and $pressureOutlet = '0.0'">
                <xsl:value-of select="func:FormatValString(string($pressureIn20), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($pressureOut20), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureIn20), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureOut20), '0.0' )"/>
                <xsl:text>;</xsl:text>
              </xsl:when>
              <!--
        IF  @P_VX20 = @P_VYX20 AND @P_VX <> @P_VYX
        BEGIN
           SET @P_1 = @P_VX20
           SET @P_2 = @P_VYX20         
           SET @T_1 = @DPBX
           SET @T_2 = @DPBYX         
        END -->
              <xsl:when test="$pressureIn20 = $pressureOut20 and $pressureInlet != $pressureOutlet">
                <xsl:value-of select="func:FormatValString(string($pressureIn20), '0.0' )"/>
                <xsl:text>;</xsl:text>


                <xsl:value-of select="func:FormatValString(string($pressureOut20), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureIn20), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureOut20), '0.0' )"/>
                <xsl:text>;</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <!--            
        SET @P_1 = @P_VX
        SET @P_2 = @P_VYX
        SET @T_1 = @T_VX
        SET @T_2 = @T_AVO-->                
                <xsl:value-of select="func:FormatValString(string($pressureInlet), '0.0' )"/>
                <xsl:text>;</xsl:text>
                
                <xsl:value-of select="func:FormatValString(string($pressureOutlet), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureInlet), '0.0' )"/>
                <xsl:text>;</xsl:text>

                <xsl:value-of select="func:FormatValString(string($temperatureCooling), '0.0' )"/>
                <xsl:text>;</xsl:text>
              </xsl:otherwise>
            </xsl:choose>                
            
            <xsl:variable name="compressorUnitsInUse" select="../Property[SysName='compressorUnitsInUse']/Value"/>
            <xsl:if test="$compressorUnitsInUse &gt; '0'">
              <xsl:value-of select="$compressorUnitsInUse"/>
            </xsl:if>
            <xsl:text>;</xsl:text>

            <xsl:variable name="compressorUnitsInReserve" select="../Property[SysName='compressorUnitsInReserve']/Value"/>
            <xsl:if test="$compressorUnitsInReserve &gt; '0'">            
              <xsl:value-of select="$compressorUnitsInReserve"/>
            </xsl:if>
            <xsl:text>;</xsl:text>
          </xsl:if>
          <!-- Обработка тип 2-->
          <xsl:if test="substring-after(ExtKey, ':') = '2'">

            <xsl:variable name="pressureInlet" select="../Property[SysName='pressureInlet']/Value"/>
            <xsl:value-of select="func:FormatValString(string($pressureInlet), '0.0' )"/>
            <xsl:text>;</xsl:text>

            <xsl:variable name="temperatureInlet" select="../Property[SysName='temperatureInlet']/Value"/>
            <xsl:value-of select="func:FormatValString(string($temperatureInlet), '0.0' )"/>
            <xsl:text>;</xsl:text>

            <xsl:if test="Value  &lt; 999.0">
              <xsl:value-of select="func:FormatValString(string(Value), '0.0' )"/>
            </xsl:if>

            <xsl:variable name="dewPoint" select="../../../../MeasurePoints/MeasurePoint[Item/ParentId=current()/../../Item/Id]/Properties/Property[SysName='dewPoint']/Value"/>
            <xsl:if test="$dewPoint  &lt; 999.0">
              <xsl:value-of select="func:FormatValString(string($dewPoint), '0.0' )"/>
            </xsl:if>
            <xsl:text>;</xsl:text>

            <!-- Добавить расход по ГРС 28:2 вычесть ГРС-->
            <xsl:choose>
              <xsl:when test="ExtKey = '28:2'">
                <xsl:variable name="GRS" select="sum(../../../../DistributingStations/DistributingStation/Properties/Property[SysName='flow']/Value)"/>
                <xsl:variable name="flow" select="(../Property[SysName='flow']/Value -$GRS) * 1000"/>

                <xsl:value-of select="func:FormatValString(string($flow), '0' )"/>

                <xsl:text>;</xsl:text>
                <xsl:text>&#13;&#10;29;</xsl:text>
                <xsl:value-of select="$timestamp" />
                <xsl:text>;;</xsl:text>
                <xsl:text>&#13;&#10;30;</xsl:text>
                <xsl:value-of select="$timestamp" />
                <xsl:text>;</xsl:text>
              </xsl:when>
              <xsl:otherwise>
                <xsl:variable name="flow" select="../Property[SysName='flow']/Value * 1000"/>

                <xsl:value-of select="func:FormatValString(string($flow), '0' )"/>
              </xsl:otherwise>
            </xsl:choose>
            <xsl:text>;</xsl:text>

          </xsl:if>
          <!-- Обработка тип 3-->
          <xsl:if test="substring-after(ExtKey, ':') = '3'">
            <xsl:variable name="temperatureAir" select="../Property[SysName='temperatureAir']/Value"/>
            <xsl:value-of select="func:FormatValString(string($temperatureAir), '0.0' )"/>
            <xsl:text>;</xsl:text>
            <xsl:value-of select="func:FormatValString(string(Value), '0.00' )"/>
            <xsl:text>;</xsl:text>
          </xsl:if>
          <xsl:text>&#13;&#10;</xsl:text>
        </xsl:if>

      </xsl:for-each>
    </xsl:if>
    <xsl:text>END</xsl:text>
  </xsl:template>
</xsl:stylesheet>
