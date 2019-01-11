<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="4.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:integro="integro"
extension-element-prefixes="integro"
>
  <!--
  <xsl:import href="Funcs.xsl"/>
  -->

  <xsl:output method="text" indent="yes" encoding="utf-16"/>


  <xsl:template match="/BusinessMessage">
      <xsl:apply-templates select="DataSection" />
  </xsl:template>
  <xsl:template match="DataSection">
      <xsl:value-of select='concat(Identifier,",&apos;",substring(concat(Value, "                    "), 1, 20),"&apos;&#13;")' />
  </xsl:template>

</xsl:stylesheet>
