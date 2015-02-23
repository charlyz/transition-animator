<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:wf="http://schemas.microsoft.com/2003/xaml/" version="1.0">
  <xsl:output method="xml" omit-xml-declaration="yes" />
  <xsl:template match="*|/">
    <wf:Form Text="Form1" Name="Form1" ClientSize="400, 400"
    xmlns="http://schemas.microsoft.com/2003/xaml/" xmlns:def="Definition" xmlns:wf="wf"
    def:Class="XamlonApplication1.Form1" def:CodeBehind="Form1.xaml.vb">
      <xsl:apply-templates select="/cuiModel/window"/>
    </wf:Form>
  </xsl:template>
  <xsl:template match="window">
    <wf:Form.Controls>
      <xsl:apply-templates select="/cuiModel/window/box/radioButton"/>
      <xsl:apply-templates select="/cuiModel/window/box/button"/>
      <xsl:apply-templates select="/cuiModel/window/box/outputText"/>
    </wf:Form.Controls>
  </xsl:template>
  <xsl:template match="radioButton">
    <wf:RadioButton Text="{@defaultContent}" TabIndex="3" Name="{@name}"/>
  </xsl:template>
  <xsl:template match="button">
    <wf:Button Text="{@defaultContent}" TabIndex="2" Name="{@name}"/>
  </xsl:template>
  <xsl:template match="outputText ">
    <wf:Label Text="{@defaultContent}" TabIndex="4" Name="{@name}"/>
  </xsl:template>
</xsl:stylesheet>