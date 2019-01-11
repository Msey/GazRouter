<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"  encoding="windows-1251" />

  <xsl:decimal-format NaN=" " />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>
  
  <xsl:template match="ExchangeMessage/DataSection">
    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
      <xsl:for-each select="DistributingStations/DistributingStation">
        <!-- Ид ГРС во внешней системе , вида [3021]-->
        
        <xsl:text>[</xsl:text>
        <xsl:value-of select="ExtKey" />
        <xsl:text>]&#09;</xsl:text> <!-- символ табуляции-->
        
        <!-- Наименование ГРС-->

        <xsl:variable name="len_name" select="string-length(Item/Name)"/>
        <xsl:choose>
              <xsl:when test="$len_name &lt; 27">
                <xsl:value-of select="Item/Name"/>
                <xsl:sequence select="string-join(for $i in 1 to (27-$len_name) return '&#32;','')"/> <!-- добавление нужного числа пробелов в конце записи имени ГРС-->
              </xsl:when>
              <xsl:otherwise>
                  <xsl:value-of select="Item/Name"/>
              </xsl:otherwise>
        </xsl:choose>
        <xsl:text>:</xsl:text>
        
        <!-- Давление на входе ГРС-->
         
         <xsl:variable name="value_pvh" select="format-number( Properties/Property[SysName='pressureInlet']/Value ,'#0.0' ) "/>
         <xsl:variable name="len_value_pvh" select=" string-length($value_pvh) "/>
         <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_pvh) return '&#32;',$value_pvh ) ,'')"/>
        
        <!-- Давление на выходе ГРС -->
         
         <xsl:variable name="value_pvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
         <xsl:variable name="len_value_pvih" select=" string-length($value_pvih) "/>
         <xsl:sequence select="string-join(   (for $k in 1 to (4 - $len_value_pvih) return '&#32;',$value_pvih ) ,'')"/>
       
        <!-- Температура на входе ГРС-->
        
         <xsl:variable name="value_tvh" select="format-number( Properties/Property[SysName='temperatureInlet']/Value ,'#0.0') "/>
         <xsl:variable name="len_value_tvh" select=" string-length($value_tvh) "/>
       <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_tvh) return '&#32;',$value_tvh ) ,'')"/>
        
        <!-- Температура на выходе ГРС-->
      
         <xsl:variable name="value_tvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='temperatureOutlet']/Value ,'#0.0' ) "/>
         <xsl:variable name="len_value_tvih" select=" string-length($value_tvih) "/>
         <xsl:sequence select="string-join(   (for $i in 1 to (5 - $len_value_tvih) return '&#32;',$value_tvih ) ,'')"/>
      
        <!-- Расход газа на ГРС-->
        
        <xsl:variable name="value_Q" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
         <xsl:variable name="len_value_Q" select=" string-length($value_Q) "/>
         <xsl:sequence select="string-join(   (for $i in 1 to (10 - $len_value_Q) return '&#32;',$value_Q ) ,'')"/>
       
        <xsl:text>&#13;&#10;</xsl:text>
      </xsl:for-each>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>

<!--
[3021]	ГРС Богословская ТЭЦ       : 39.7  6.0  19.0   3.0     76.400
[3024]	ГРС Серовская птицефабрика : 39.6  4.6  30.0  16.0      0.257
[3014]	ГРС Воронцовский ГОК       : 66.4  6.0  44.0  14.7      0.704
[3019]	ГРС г. Волчанский          : 40.0  6.2  28.0   7.1      7.780
[28 1]	АГРС Ивдель (Урожай 20)    : 35.5  5.7   0.9  10.0      2.443
[28 5]	ГРС ст.Ивдель I -          : 35.1  3.3  -1.3  14.0      0.347
[3246]	ГРС г. Качканар            : 41.0  7.6   7.0 -12.5     33.170
[3020]	ГРС г. Краснотурьинск      : 38.5  6.0  24.0  10.0     50.700
[3240]	ГРС г. Красноуральск       : 33.5  5.2   8.7 -12.6     12.800
[3245]	ГРС г. Н.Тура              : 42.5  5.7   8.0  -7.0     13.864
[3018]	ГРС г. Североуральск       : 39.7  6.2  25.0  10.0     13.290
[3023]	ГРС г. Серов               : 39.0 12.0   6.0 -10.8    115.000
[33 1]	ГРС п. ИС                  : 59.9  5.1  28.4   3.4      1.331
[3025]	ГРС г. Лобва               : 38.5  6.0  37.0   8.0      0.570
[32 5]	ГРС Новолялинский ЦБК      : 38.0  5.9   0.7   5.7      5.118
[30 2]	ГРС Северопесчанская       : 39.7  6.0   1.0 -13.0      1.584
-->
