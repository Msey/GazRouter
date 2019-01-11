<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="2.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema"
  xmlns:func="http://MyFunction"
	xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="text"   encoding="windows-1251" />

  <xsl:decimal-format NaN="0" />

  <xsl:template match="text()">
    <xsl:apply-templates />
  </xsl:template>

  <xsl:variable name="comma_separator" >,</xsl:variable>
  <xsl:variable name="line_separator" >
    <xsl:text>&#13;&#10;</xsl:text>
  </xsl:variable>


  <xsl:template match="ExchangeMessage/HeaderSection">
    <xsl:variable name="timestamp" as ="xs:dateTime">
      <xsl:value-of select="TimeStamp" />
    </xsl:variable>
    <xsl:value-of select="format-dateTime($timestamp, ',[Y01].[M01].[D01]')" />
    <xsl:value-of select="$comma_separator"/>
    <xsl:text>Часовая реализация газа по ГРС П."ЮГТРАНСГАЗ",,,,,,,, </xsl:text>
    <xsl:value-of select="format-dateTime($timestamp, '[H1]:00')" />
    <xsl:value-of select="$line_separator"/>
    <xsl:text>Код,ЛПУ,ГРС,Факт,Лимит,Откл.,Рвх.,Рвых.,Рпотр.,Твх.,Твых.</xsl:text>
    <xsl:value-of select="$line_separator"/>    
  </xsl:template>

  <xsl:template match="ExchangeMessage/DataSection">

    <xsl:if test="count(DistributingStations/DistributingStation) &gt; 0">
      <xsl:for-each select="DistributingStations/DistributingStation">
        <!-- Сортировка Ид ГРС-->
        <xsl:sort select="ExtKey"/>
        <!-- Ид ГРС во внешней системе , вида [0111701{L0100001}]-->

        <!-- ЛПУ -->
        <!-- Доработать получить ExtKey от ЛПУ-->
        
        <xsl:variable name="value_LPU" select="upper-case(substring-before(substring-after(Item/Path, '/'), '/'))"/>
        <xsl:variable name="ExtKeyLPUString" select="substring(ExtKey,9)"/>
        <xsl:if test="substring(ExtKey,8,1) = 'L'">
          <xsl:text>"</xsl:text>
          <xsl:value-of select="$ExtKeyLPUString"/>
          <xsl:text>"</xsl:text>
          <xsl:value-of select="$comma_separator"/>
          <xsl:text>"</xsl:text>
          <xsl:value-of select="$value_LPU"/>
          <xsl:text>"</xsl:text>
          <xsl:text>,,0.000,0.000,0.000,0.0,0.0,0.0,0.0,0.0,</xsl:text>
          <xsl:value-of select="$line_separator"/>
        </xsl:if>

        <!-- Наименование ГРС-->
        <xsl:variable name="ExtKeyString" select="substring(ExtKey,1,7)"/>
        <xsl:text>"</xsl:text>
        <xsl:value-of select="$ExtKeyString"/>
        <xsl:text>"</xsl:text>
        <xsl:value-of select="$comma_separator"/>
        <xsl:value-of select="$comma_separator"/>
        
        <xsl:variable name="grs_name" select="Item/Name"/>
        
        <xsl:text>"</xsl:text>
        <xsl:value-of select="$grs_name"/>
        <xsl:text>"</xsl:text>        
        <xsl:value-of select="$comma_separator"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- Расход газа на ГРС-->

        <xsl:variable name="value_Q" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
        <xsl:value-of select="$value_Q"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- План Расход газа на ГРС-->
        <!-- Доработать -->

        <xsl:variable name="value_PQ" select="format-number( Properties/Property[SysName='flow']/Value  ,'#0.000')"/>
        <xsl:value-of select="$value_PQ"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- Отклонение Расход газа на ГРС-->

        <xsl:variable name="value_dQ" select="format-number(number($value_PQ) - number($value_Q),'#0.000')"/>
        <xsl:value-of select="$value_dQ"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- Давление на входе ГРС-->

        <xsl:variable name="value_pvh" select="format-number( Properties/Property[SysName='pressureInlet']/Value ,'#0.0' ) "/>
        <xsl:value-of select="$value_pvh"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- Давление на выходе ГРС -->

        <xsl:variable name="value_pvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
        <xsl:value-of select="value_pvih"/>
        <xsl:value-of select="$comma_separator"/>

        <!-- Давление на потребителе -->
        <!-- Доработать -->

        <xsl:variable name="value_pot" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='pressureOutlet']/Value,'#0.0' ) "/>
        <xsl:value-of select="$value_pot"/>
        <xsl:value-of select="$comma_separator"/>        

        <!-- Температура на входе ГРС-->

        <xsl:variable name="value_tvh" select="format-number( Properties/Property[SysName='temperatureInlet']/Value ,'#0.0') "/>
        <xsl:value-of select="$value_tvh"/>
        <xsl:value-of select="$comma_separator"/>        

        <!-- Температура на выходе ГРС-->

        <xsl:variable name="value_tvih" select="format-number( ../../Outputs/Output[Item/ParentId=current()/Item/Id]/Properties/Property[SysName='temperatureOutlet']/Value ,'#0.0' ) "/>
        <xsl:value-of select="$value_tvih"/>
        <xsl:value-of select="$comma_separator"/>

        <xsl:value-of select="$line_separator"/>
        <!-- Добавить информацию по потребителям -->
        <!-- Доработать -->
      </xsl:for-each>

      <!-- Добавление последней строки-->
      <xsl:text>"2010000","САРАТОВСКАЯ ОБЛ."</xsl:text>
      <xsl:value-of select="$comma_separator"/>
      <xsl:value-of select="$comma_separator"/>      

      <!-- Расход газа по всем ГРС-->

      <xsl:variable name="value_TQ" select="format-number( sum(DistributingStations/DistributingStation/Properties/Property[SysName='flow']/Value)  ,'#0.000')"/>
      <xsl:value-of select="$value_TQ"/>
      <xsl:value-of select="$comma_separator"/>

      <!-- План Расход газа по всем  ГРС-->
      <!-- Доработать -->

      <xsl:variable name="value_TPQ" select="format-number( sum(DistributingStations/DistributingStation/Properties/Property[SysName='flow']/Value)  ,'#0.000')"/>
      <xsl:value-of select="$value_TPQ"/>
      <xsl:value-of select="$comma_separator"/>

      <!-- Отклонение Расход газа по всем  ГРС-->

      <xsl:variable name="value_TdQ" select="format-number(number($value_TPQ) - number($value_TQ),'#0.000')"/>
      <xsl:value-of select="$value_TdQ"/>
      <xsl:text>,0.0,0.0,0.0,0.0,0.0</xsl:text>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>