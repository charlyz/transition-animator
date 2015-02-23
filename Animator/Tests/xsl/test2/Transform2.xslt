<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
	    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <xsl:template match="*|@*">
    <xsl:copy>
      <xsl:apply-templates select="@*[not(.='')]"/>
      <xsl:apply-templates select="node()"/>
    </xsl:copy>
  </xsl:template>
  

</xsl:transform>
