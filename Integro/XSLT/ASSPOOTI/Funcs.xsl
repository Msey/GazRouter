<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 
xmlns:integro="integro"
xmlns:func="http://exslt.org/functions"
extension-element-prefixes="func"
>
  <xsl:function name="integro:pad-string-to-length" as="xs:string">
    <xsl:param name="stringToPad" as="xs:string?"/>
    <xsl:param name="padChar" as="xs:string"/>
    <xsl:param name="length" as="xs:integer"/>

  <xsl:sequence select="
   substring(
     string-join (
       ($stringToPad, for $i in (1 to $length) return $padChar)
       ,'')
    ,1,$length)
  "/>
  </xsl:function>

</xsl:stylesheet>
