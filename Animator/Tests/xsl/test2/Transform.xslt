<?xml version="1.0" encoding="UTF-8"?>
<xsl:transform xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes" />

  <xsl:template match="/">
    <xsl:apply-templates select="/uiModel/cuiModel/window"/>
  </xsl:template>

  <xsl:template match="window">

    <xsl:element name="Window" use-attribute-sets="TwoDgraphicalContainer" >

      <!--
        Champs sans correspondance ou non gérés
          - windowLeftMargin
          - windowRightMargin
        
        -->

      <xsl:choose>
        <xsl:when test="@isResizable = 'true'">
          <xsl:attribute name="ResizeMode">
            <xsl:value-of select="'CanResize'" />
          </xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="ResizeMode">
            <xsl:value-of select="'NoResize'" />
          </xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>

      <!--
          Changement de la longueur et de la largeur
          de la fenetre car Windows compte les bordures
          dans la définition de la taille des fenetres.
        -->
      <xsl:if test="@width != ''">
        <xsl:attribute name="Width">
          <xsl:value-of select="@width + 90" />
        </xsl:attribute>
      </xsl:if>
      <xsl:if test="@height != ''">
        <xsl:attribute name="Height">
          <xsl:value-of select="@height + 40" />
        </xsl:attribute>
      </xsl:if>

      <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

      <xsl:if test="@transparencyRate != ''">
        <xsl:attribute name="Opacity">
          <xsl:value-of select="@transparencyRate div 100" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@isVisible != ''">
        <xsl:choose>
          <xsl:when test="@isVisible = 'false'">
            <xsl:attribute name="Visibility">
              <xsl:value-of select="'Hidden'" />
            </xsl:attribute>
          </xsl:when>
          <xsl:otherwise>
            <xsl:attribute name="Visibility">
              <xsl:value-of select="'Visible'" />
            </xsl:attribute>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>

      <xsl:apply-templates />

    </xsl:element>
  </xsl:template>

  <xsl:template match="box" >

    <xsl:element name="Grid" use-attribute-sets="TwoDgraphicalContainer" >

      <!--
        Champs sans correspondance ou non gérés
          - isSplitable
          - isBalanced
          - isResizableHorizontal
          - isResizableVertical
          - relativeMinWidth
          - relativeMinHeight
          - isScrollable
        -->

      <xsl:if test="name(parent::node()) = 'window'">
        <xsl:attribute name="Name">
          <xsl:value-of select="''" />
        </xsl:attribute>
        <xsl:attribute name="x:Name">
          <xsl:value-of select="@name" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@relativeWidth != '' and @width = '' and name(parent::node()) != 'box' and parent::node()/@width != ''">
        <xsl:attribute name="Width">
          <xsl:value-of select="@relativeWidth div 100 * parent::node()/@width" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@relativeHeight != '' and @height = '' and name(parent::node()) != 'box' and parent::node()/@height != ''">
        <xsl:attribute name="Height">
          <xsl:value-of select="@relativeHeight div 100 * parent::node()/@height" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@isEnabled != ''">
        <xsl:attribute name="IsEnabled">
          <xsl:value-of select="@isEnabled" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@width != ''">
        <xsl:attribute name="Width">
          <xsl:value-of select="@width" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@height != ''">
        <xsl:attribute name="Height">
          <xsl:value-of select="@height" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>

      <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

      <xsl:if test="@transparencyRate != ''">
        <xsl:attribute name="Opacity">
          <xsl:value-of select="@transparencyRate div 100" />
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="@isVisible != ''">
        <xsl:choose>
          <xsl:when test="@isVisible = 'false'">
            <xsl:attribute name="Visibility">
              <xsl:value-of select="'Hidden'" />
            </xsl:attribute>
          </xsl:when>
          <xsl:otherwise>
            <xsl:attribute name="Visibility">
              <xsl:value-of select="'Visible'" />
            </xsl:attribute>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:if>


      <!--
        Champs hérités n'ayant pas de correspondance. On surchage le champs
        par une string vide pour supprimer le champs.
        
      -->
      <xsl:attribute name="BorderThickness">
        <xsl:value-of select="''" />
      </xsl:attribute>

      <xsl:attribute name="BorderBrush">
        <xsl:value-of select="''" />
      </xsl:attribute>

      <xsl:attribute name="IsEnabled">
        <xsl:value-of select="''" />
      </xsl:attribute>

      <!--
      
      Définition des colonnes et des lignes à la fin
      car sinon elle n'est plus possible de surcharger
      les attributs hérités, un bug surement.
      
      -->
      <xsl:choose>
        <xsl:when test="@type = 'vertical'">
          <Grid.RowDefinitions>
            <xsl:call-template name="counterToShowGridDef">
              <xsl:with-param name="iteration" select="0"/>
              <xsl:with-param name="end" select="count(current()/child::*)"/>
              <xsl:with-param name="value" select="'row'"/>
            </xsl:call-template>
          </Grid.RowDefinitions>
        </xsl:when>
        <xsl:otherwise>
          <Grid.ColumnDefinitions>
            <xsl:call-template name="counterToShowGridDef">
              <xsl:with-param name="iteration" select="0"/>
              <xsl:with-param name="end" select="count(current()/child::*)"/>
              <xsl:with-param name="value" select="'col'"/>
            </xsl:call-template>
          </Grid.ColumnDefinitions>
        </xsl:otherwise>
      </xsl:choose>



      <xsl:apply-templates />

    </xsl:element>

    <!--<xsl:value-of select="@name" /> -
    <xsl:number value="position()" format="1" />
    précédent: <xsl:value-of select="count(.. | ../preceding-sibling::box)"/>
     -->
  </xsl:template>

  <xsl:template match="outputText" >

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:element name="TextBlock"  use-attribute-sets="TwoDgraphicalIndividualComponent">

        <!--
        Champs sans correspondance ou non gérés
          - defaultHyperLinkTarget
          - hyperLinkTarget
          - visitedLinkColor
          - activeLinkColor
          - scrollStyle
          - scrollDirection
          - scrollHeight
          - scrollWidth
          - scrollHorizontalSpace
          - scrollVerticalSpace
          - scrollAmount
          - scrollDelay
          - numberOfColumns
          - numberOfLines
          - defaultFilter
          - filter
        -->

        <xsl:if test="@textMargin != ''">
          <xsl:attribute name="Margin">
            <xsl:value-of select="@textMargin" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@textVerticalAlign != ''">
          <xsl:attribute name="VerticalAlignment">
            <xsl:choose>
              <xsl:when test="@textVerticalAlign = 'top'">
                <xsl:value-of select="'Top'" />
              </xsl:when>
              <xsl:when test="@textVerticalAlign = 'bottom'">
                <xsl:value-of select="'Bottom'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Stretch'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@textHorizontalAlign != ''">
          <xsl:attribute name="HorizontalAlignment">
            <xsl:choose>
              <xsl:when test="@textHorizontalAlign = 'right'">
                <xsl:value-of select="'Right'" />
              </xsl:when>
              <xsl:when test="@textHorizontalAlign = 'middle'">
                <xsl:value-of select="'Center'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Left'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@defaultContent != ''">
          <xsl:attribute name="Text">
            <xsl:value-of select="@defaultContent" />
          </xsl:attribute>
        </xsl:if>


        <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

        <xsl:if test="@borderTitleAlign != ''">
          <xsl:attribute name="FlowDirection">
            <xsl:choose>
              <xsl:when test="@borderTitleAlign = 'right'">
                <xsl:value-of select="'RightToLeft'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'LeftToRight'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isBold != ''">
          <xsl:attribute name="FontWeight">
            <xsl:choose>
              <xsl:when test="@isBold = 'true'">
                <xsl:value-of select="'Bold'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isItalic != ''">
          <xsl:attribute name="FontStyle">
            <xsl:choose>
              <xsl:when test="@isItalic = 'true'">
                <xsl:value-of select="'Italic'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isUnderlined != ''">
          <xsl:attribute name="TextDecorations">
            <xsl:choose>
              <xsl:when test="@isUnderlined = 'true'">
                <xsl:value-of select="'Underline'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="''" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@transparencyRate != ''">
          <xsl:attribute name="Opacity">
            <xsl:value-of select="@transparencyRate div 100" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isVisible != ''">
          <xsl:choose>
            <xsl:when test="@isVisible = 'false'">
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Hidden'" />
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Visible'" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>

        <!--
        Champs hérités n'ayant pas de correspondance. On surchage le champs
        par une string vide pour supprimer le champs.
       
      -->

        <xsl:attribute name="BorderThickness">
          <xsl:value-of select="''" />
        </xsl:attribute>

        <xsl:attribute name="BorderBrush">
          <xsl:value-of select="''" />
        </xsl:attribute>

        <xsl:attribute name="TopMost">
          <xsl:value-of select="''" />
        </xsl:attribute>

        <xsl:attribute name="IsEnabled">
          <xsl:value-of select="''" />
        </xsl:attribute>

      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="inputText" >

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:choose>
        <xsl:when test="not(@isPassword = 'true')">
          <xsl:element name="TextBox"  use-attribute-sets="TwoDgraphicalIndividualComponent">
            <!--
            Champs sans correspondance ou non gérés
              - forceWordWrapped
              - numberOfColumns
              - numberOfLines
              - filter
              - defaultFilter
            -->

            <xsl:if test="@isEditable != ''">
              <xsl:attribute name="IsReadOnly">
                <xsl:choose>
                  <xsl:when test="@isEditable = 'true'">
                    <xsl:value-of select="'false'" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="'true'" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@isWordWrapped != ''">
              <xsl:attribute name="WordWrap">
                <xsl:value-of select="@isWordWrapped" />
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@maxLength != ''">
              <xsl:attribute name="MaxLength">
                <xsl:value-of select="@maxLength" />
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@textVerticalAlign != ''">
              <xsl:attribute name="VerticalAlignment">
                <xsl:choose>
                  <xsl:when test="@textVerticalAlign = 'top'">
                    <xsl:value-of select="'Top'" />
                  </xsl:when>
                  <xsl:when test="@textVerticalAlign = 'bottom'">
                    <xsl:value-of select="'Bottom'" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="'Stretch'" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@textHorizontalAlign != ''">
              <xsl:attribute name="HorizontalAlignment">
                <xsl:choose>
                  <xsl:when test="@textHorizontalAlign = 'right'">
                    <xsl:value-of select="'Right'" />
                  </xsl:when>
                  <xsl:when test="@textHorizontalAlign = 'center'">
                    <xsl:value-of select="'Center'" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="'Left'" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@defaultContent != ''">
              <xsl:attribute name="Text">
                <xsl:value-of select="@defaultContent" />
              </xsl:attribute>
            </xsl:if>

            <!--
      
            Champs à ajouter manuellement à cause de la conversion des valeurs
            (utilisation de IF non autorisé dans attribute-set)

            -->

            <xsl:if test="@isVisible != ''">
              <xsl:choose>
                <xsl:when test="@isVisible = 'false'">
                  <xsl:attribute name="Visibility">
                    <xsl:value-of select="'Hidden'" />
                  </xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:attribute name="Visibility">
                    <xsl:value-of select="'Visible'" />
                  </xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:if>

            <!--
            Champs hérités n'ayant pas de correspondance. On surchage le champs
            par une string vide pour supprimer le champs.
        
            -->

            <xsl:attribute name="BorderThickness">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="BorderBrush">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="TopMost">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="FlowDirection">
              <xsl:value-of select="''" />
            </xsl:attribute>

          </xsl:element>
        </xsl:when>
        <xsl:otherwise>
          <xsl:element name="PasswordBox"  use-attribute-sets="TwoDgraphicalIndividualComponent">
            <!--
            Champs sans correspondance ou non gérés
              - forceWordWrapped
              - numberOfColumns
              - numberOfLines
              - filter
              - defaultFilter
            -->

            <xsl:if test="@isWordWrapped != ''">
              <xsl:attribute name="WordWrap">
                <xsl:value-of select="@isWordWrapped" />
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@maxLength != ''">
              <xsl:attribute name="MaxLength">
                <xsl:value-of select="@maxLength" />
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@textVerticalAlign != ''">
              <xsl:attribute name="VerticalAlignment">
                <xsl:choose>
                  <xsl:when test="@textVerticalAlign = 'top'">
                    <xsl:value-of select="'Top'" />
                  </xsl:when>
                  <xsl:when test="@textVerticalAlign = 'bottom'">
                    <xsl:value-of select="'Bottom'" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="'Stretch'" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@textHorizontalAlign != ''">
              <xsl:attribute name="HorizontalAlignment">
                <xsl:choose>
                  <xsl:when test="@textHorizontalAlign = 'right'">
                    <xsl:value-of select="'Right'" />
                  </xsl:when>
                  <xsl:when test="@textHorizontalAlign = 'center'">
                    <xsl:value-of select="'Center'" />
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:value-of select="'Left'" />
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
            </xsl:if>

            <xsl:if test="@defaultContent != ''">
              <xsl:attribute name="Password">
                <xsl:value-of select="@defaultContent" />
              </xsl:attribute>
            </xsl:if>

            <!--
      
            Champs à ajouter manuellement à cause de la conversion des valeurs
            (utilisation de IF non autorisé dans attribute-set)

            -->

            <xsl:if test="@isVisible != ''">
              <xsl:choose>
                <xsl:when test="@isVisible = 'false'">
                  <xsl:attribute name="Visibility">
                    <xsl:value-of select="'Hidden'" />
                  </xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:attribute name="Visibility">
                    <xsl:value-of select="'Visible'" />
                  </xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
            </xsl:if>

            <!--
            Champs hérités n'ayant pas de correspondance. On surchage le champs
            par une string vide pour supprimer le champs.
        
          -->

            <xsl:attribute name="BorderThickness">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="BorderBrush">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="TopMost">
              <xsl:value-of select="''" />
            </xsl:attribute>

            <xsl:attribute name="FlowDirection">
              <xsl:value-of select="''" />
            </xsl:attribute>

          </xsl:element>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:element>
  </xsl:template>

  <xsl:template match="button">

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:element name="Button"  use-attribute-sets="TwoDgraphicalIndividualComponent">

        <!--
      
        Champs à ajouter manuellement à cause de la conversion des valeurs
        (utilisation de IF non autorisé dans attribute-set)

        -->

        <xsl:if test="@defaultContent != ''">
          <xsl:attribute name="Content">
            <xsl:value-of select="@defaultContent" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@borderTitleAlign != ''">
          <xsl:attribute name="FlowDirection">
            <xsl:choose>
              <xsl:when test="@borderTitleAlign = 'right'">
                <xsl:value-of select="'RightToLeft'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'LeftToRight'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isBold != ''">
          <xsl:attribute name="FontWeight">
            <xsl:choose>
              <xsl:when test="@isBold = 'true'">
                <xsl:value-of select="'Bold'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isItalic != ''">
          <xsl:attribute name="FontStyle">
            <xsl:choose>
              <xsl:when test="@isItalic = 'true'">
                <xsl:value-of select="'Italic'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isUnderlined != ''">
          <xsl:attribute name="TextDecorations">
            <xsl:choose>
              <xsl:when test="@isUnderlined = 'true'">
                <xsl:value-of select="'Underline'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="''" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@transparencyRate != ''">
          <xsl:attribute name="Opacity">
            <xsl:value-of select="@transparencyRate div 100" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isVisible != ''">
          <xsl:choose>
            <xsl:when test="@isVisible = 'false'">
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Hidden'" />
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Visible'" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>

        <xsl:attribute name="HorizontalAlignment">
          <xsl:value-of select="'Center'" />
        </xsl:attribute>

        <xsl:attribute name="VerticalAlignment">
          <xsl:value-of select="'Center'" />
        </xsl:attribute>

        <!--
        Champs hérités n'ayant pas de correspondance. On surchage le champs
        par une string vide pour supprimer le champs.
      
      -->

        <xsl:attribute name="TopMost">
          <xsl:value-of select="''" />
        </xsl:attribute>

      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="radioButton">

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>
      <xsl:element name="RadioButton"  use-attribute-sets="TwoDgraphicalIndividualComponent">


        <xsl:if test="@groupName != ''">
          <xsl:attribute name="GroupName">
            <xsl:value-of select="@groupName" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@defaultState != ''">
          <xsl:attribute name="IsChecked">
            <xsl:value-of select="@defaultState" />
          </xsl:attribute>
        </xsl:if>

        <!--
      
        Champs à ajouter manuellement à cause de la conversion des valeurs
        (utilisation de IF non autorisé dans attribute-set)

        -->

        <xsl:if test="@defaultContent != ''">
          <xsl:attribute name="Content">
            <xsl:value-of select="@defaultContent" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isBold != ''">
          <xsl:attribute name="FontWeight">
            <xsl:choose>
              <xsl:when test="@isBold = 'true'">
                <xsl:value-of select="'Bold'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isItalic != ''">
          <xsl:attribute name="FontStyle">
            <xsl:choose>
              <xsl:when test="@isItalic = 'true'">
                <xsl:value-of select="'Italic'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isUnderlined != ''">
          <xsl:attribute name="TextDecorations">
            <xsl:choose>
              <xsl:when test="@isUnderlined = 'true'">
                <xsl:value-of select="'Underline'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="''" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@transparencyRate != ''">
          <xsl:attribute name="Opacity">
            <xsl:value-of select="@transparencyRate div 100" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isVisible != ''">
          <xsl:choose>
            <xsl:when test="@isVisible = 'false'">
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Hidden'" />
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Visible'" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>

        <!--
        Champs hérités n'ayant pas de correspondance. On surchage le champs
        par une string vide pour supprimer le champs.
      
      -->

        <xsl:attribute name="TopMost">
          <xsl:value-of select="''" />
        </xsl:attribute>

      </xsl:element>
    </xsl:element>
  </xsl:template>


  <xsl:template match="listBox" >

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:element name="ListBox"  use-attribute-sets="TwoDgraphicalIndividualComponent">

        <!--
        Champs sans correspondance ou non gérés
          - maxlineVisible
          - multipleSelection
        -->

        <xsl:if test="@isEditable != ''">
          <xsl:attribute name="IsEditable">
            <xsl:value-of select="@isEditable" />
          </xsl:attribute>
        </xsl:if>

        <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

        <xsl:if test="@borderTitleAlign != ''">
          <xsl:attribute name="FlowDirection">
            <xsl:choose>
              <xsl:when test="@borderTitleAlign = 'right'">
                <xsl:value-of select="'RightToLeft'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'LeftToRight'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isBold != ''">
          <xsl:attribute name="FontWeight">
            <xsl:choose>
              <xsl:when test="@isBold = 'true'">
                <xsl:value-of select="'Bold'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isItalic != ''">
          <xsl:attribute name="FontStyle">
            <xsl:choose>
              <xsl:when test="@isItalic = 'true'">
                <xsl:value-of select="'Italic'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@transparencyRate != ''">
          <xsl:attribute name="Opacity">
            <xsl:value-of select="@transparencyRate div 100" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isVisible != ''">
          <xsl:choose>
            <xsl:when test="@isVisible = 'false'">
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Hidden'" />
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Visible'" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>

        <xsl:apply-templates />

      </xsl:element>
    </xsl:element>



  </xsl:template>

  <xsl:template match="comboBox" >

    <xsl:element name="Grid">

      <xsl:attribute name="Name">
        <xsl:value-of select="generate-id()"/>
      </xsl:attribute>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'vertical']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="0"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:if test="name(parent::node()) = 'box' and parent::node()[@type = 'horizontal']">
        <xsl:attribute name="Grid.Column">
          <xsl:value-of select="position() - 1"/>
        </xsl:attribute>
        <xsl:attribute name="Grid.Row">
          <xsl:value-of select="0"/>
        </xsl:attribute>
      </xsl:if>

      <xsl:element name="ComboBox"  use-attribute-sets="TwoDgraphicalIndividualComponent">

        <!-- <xsl:attribute name="SelectionChanged">
          <xsl:value-of select="'ComboBox_SelectionChanged'" />
        </xsl:attribute>-->

        <!--
        Champs sans correspondance ou non gérés
          - maxlineVisible
        -->

        <xsl:if test="@isEditable != ''">
          <xsl:attribute name="IsEditable">
            <xsl:value-of select="@isEditable" />
          </xsl:attribute>
        </xsl:if>


        <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

        <xsl:if test="@borderTitleAlign != ''">
          <xsl:attribute name="FlowDirection">
            <xsl:choose>
              <xsl:when test="@borderTitleAlign = 'right'">
                <xsl:value-of select="'RightToLeft'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'LeftToRight'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isBold != ''">
          <xsl:attribute name="FontWeight">
            <xsl:choose>
              <xsl:when test="@isBold = 'true'">
                <xsl:value-of select="'Bold'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isItalic != ''">
          <xsl:attribute name="FontStyle">
            <xsl:choose>
              <xsl:when test="@isItalic = 'true'">
                <xsl:value-of select="'Italic'" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="'Normal'" />
              </xsl:otherwise>
            </xsl:choose>
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@transparencyRate != ''">
          <xsl:attribute name="Opacity">
            <xsl:value-of select="@transparencyRate div 100" />
          </xsl:attribute>
        </xsl:if>

        <xsl:if test="@isVisible != ''">
          <xsl:choose>
            <xsl:when test="@isVisible = 'false'">
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Hidden'" />
              </xsl:attribute>
            </xsl:when>
            <xsl:otherwise>
              <xsl:attribute name="Visibility">
                <xsl:value-of select="'Visible'" />
              </xsl:attribute>
            </xsl:otherwise>
          </xsl:choose>
        </xsl:if>

        <xsl:apply-templates />

      </xsl:element>
    </xsl:element>



  </xsl:template>


  <xsl:template match="item" >


    <xsl:choose>
      <xsl:when test="name(parent::node()) = 'comboBox' or name(parent::node()) = 'listBox'">

        <xsl:choose>
          <xsl:when test="name(parent::node()) = 'comboBox'">
            <xsl:element name="ComboBoxItem"  use-attribute-sets="TwoDgraphicalIndividualComponent">

              <!--
        Champs sans correspondance ou non gérés
          - maxlineVisible
        -->

              <xsl:if test="@defaultContent != ''">
                <xsl:attribute name="Content">
                  <xsl:value-of select="@defaultContent" />
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isEditable != ''">
                <xsl:attribute name="IsEditable">
                  <xsl:value-of select="@isEditable" />
                </xsl:attribute>
              </xsl:if>


              <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

              <xsl:if test="@borderTitleAlign != ''">
                <xsl:attribute name="FlowDirection">
                  <xsl:choose>
                    <xsl:when test="@borderTitleAlign = 'right'">
                      <xsl:value-of select="'RightToLeft'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'LeftToRight'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isBold != ''">
                <xsl:attribute name="FontWeight">
                  <xsl:choose>
                    <xsl:when test="@isBold = 'true'">
                      <xsl:value-of select="'Bold'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'Normal'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isItalic != ''">
                <xsl:attribute name="FontStyle">
                  <xsl:choose>
                    <xsl:when test="@isItalic = 'true'">
                      <xsl:value-of select="'Italic'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'Normal'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@transparencyRate != ''">
                <xsl:attribute name="Opacity">
                  <xsl:value-of select="@transparencyRate div 100" />
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isVisible != ''">
                <xsl:choose>
                  <xsl:when test="@isVisible = 'false'">
                    <xsl:attribute name="Visibility">
                      <xsl:value-of select="'Hidden'" />
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="Visibility">
                      <xsl:value-of select="'Visible'" />
                    </xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:if>

            </xsl:element>
          </xsl:when>
          <xsl:otherwise>
            <xsl:element name="ListBoxItem"  use-attribute-sets="TwoDgraphicalIndividualComponent">

              <!--
        Champs sans correspondance ou non gérés
          - maxlineVisible
        -->

              <xsl:if test="@defaultContent != ''">
                <xsl:attribute name="Content">
                  <xsl:value-of select="@defaultContent" />
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isEditable != ''">
                <xsl:attribute name="IsEditable">
                  <xsl:value-of select="@isEditable" />
                </xsl:attribute>
              </xsl:if>


              <!--
      
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)

      -->

              <xsl:if test="@borderTitleAlign != ''">
                <xsl:attribute name="FlowDirection">
                  <xsl:choose>
                    <xsl:when test="@borderTitleAlign = 'right'">
                      <xsl:value-of select="'RightToLeft'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'LeftToRight'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isBold != ''">
                <xsl:attribute name="FontWeight">
                  <xsl:choose>
                    <xsl:when test="@isBold = 'true'">
                      <xsl:value-of select="'Bold'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'Normal'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isItalic != ''">
                <xsl:attribute name="FontStyle">
                  <xsl:choose>
                    <xsl:when test="@isItalic = 'true'">
                      <xsl:value-of select="'Italic'" />
                    </xsl:when>
                    <xsl:otherwise>
                      <xsl:value-of select="'Normal'" />
                    </xsl:otherwise>
                  </xsl:choose>
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@transparencyRate != ''">
                <xsl:attribute name="Opacity">
                  <xsl:value-of select="@transparencyRate div 100" />
                </xsl:attribute>
              </xsl:if>

              <xsl:if test="@isVisible != ''">
                <xsl:choose>
                  <xsl:when test="@isVisible = 'false'">
                    <xsl:attribute name="Visibility">
                      <xsl:value-of select="'Hidden'" />
                    </xsl:attribute>
                  </xsl:when>
                  <xsl:otherwise>
                    <xsl:attribute name="Visibility">
                      <xsl:value-of select="'Visible'" />
                    </xsl:attribute>
                  </xsl:otherwise>
                </xsl:choose>
              </xsl:if>

            </xsl:element>
          </xsl:otherwise>
        </xsl:choose>

      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="'Normal'" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="counterToShowGridDef">
    <xsl:param name="iteration"/>
    <xsl:param name="end"/>
    <xsl:param name="value"/>
    <xsl:if test="$iteration &lt; $end">
      <xsl:choose>
        <xsl:when test="$value = 'col'">
          <xsl:choose>
            <xsl:when test="./*[$iteration+1]/@relativeWidth != ''">
              <xsl:element name="ColumnDefinition">
                <xsl:attribute name="Width">
                  <xsl:value-of select="concat(./*[$iteration+1]/@relativeWidth div 100, '*')" />
                </xsl:attribute>
              </xsl:element>
            </xsl:when>
            <xsl:otherwise>
              <ColumnDefinition Width="1*" />
            </xsl:otherwise>
          </xsl:choose>

        </xsl:when>
        <xsl:otherwise>
          <xsl:choose>
            <xsl:when test="./*[$iteration+1]/@relativeHeight != ''">
              <xsl:element name="RowDefinition">
                <xsl:attribute name="Height">
                  <xsl:value-of select="concat(./*[$iteration+1]/@relativeHeight div 100, '*')" />
                </xsl:attribute>
              </xsl:element>
            </xsl:when>
            <xsl:otherwise>
              <RowDefinition Height="1*" />
            </xsl:otherwise>
          </xsl:choose>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:call-template name="counterToShowGridDef">
        <xsl:with-param name="iteration" select="$iteration + 1"/>
        <xsl:with-param name="end" select="$end"/>
        <xsl:with-param name="value" select="$value"/>
      </xsl:call-template>
    </xsl:if>
  </xsl:template>

  <!--
  
  Ensembles d'attributs permettant de représenter la structure
  d'héritage de cio.
  
  -->

  <xsl:attribute-set name="cio" >

    <!--
      Champs sans correspondance ou non gérés
        - id
        - icon
        - content
        - defaultIcon
        - defaultHelp
        - help
        - currentValue
        - error
        - feedBack
        - isMandatory
        
      Champs ajoutés manuellement car changeant de noms en fonction de l'élément
        - defaultContent
    -->

    <xsl:attribute name="Name">
      <xsl:value-of select="@name" />
    </xsl:attribute>


  </xsl:attribute-set>

  <!-- Remplacement de 2 par Two dans 2DgraphicalCio car le champs name ne peut commencer par un chiffre -->
  <xsl:attribute-set name="TwoDgraphicalCio" use-attribute-sets="cio">

    <!--
      Champs sans correspondance ou non gérés
        - statusBarContent 
        - defaultStatusBarContent
        - fgColor
        - borderType
        - borderTitle
        - defaultBorderTitle
        - toolTipContent
        
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)
        - borderTitleAlign (-> FlowDirection)
        - isvisible (-> Visibility)
        
    -->

    <xsl:attribute name="IsEnabled">
      <xsl:value-of select="@isEnabled" />
    </xsl:attribute>

    <xsl:attribute name="Background">
      <xsl:value-of select="@bgColor" />
    </xsl:attribute>

    <xsl:attribute name="BorderThickness">
      <xsl:value-of select="@borderWidth" />
    </xsl:attribute>

    <xsl:attribute name="BorderBrush">
      <xsl:value-of select="@borderColor" />
    </xsl:attribute>

    <xsl:attribute name="ToolTip">
      <xsl:value-of select="@toolTipDefaultContent" />
    </xsl:attribute>

  </xsl:attribute-set>

  <xsl:attribute-set name="TwoDgraphicalContainer" use-attribute-sets="TwoDgraphicalCio">

    <!--
      Champs sans correspondance ou non gérés
        - bgImage
        - repetition
        - isDetachable
        - isMigrateable
    -->

    <xsl:attribute name="Width">
      <xsl:value-of select="@width" />
    </xsl:attribute>

    <xsl:attribute name="Height">
      <xsl:value-of select="@height" />
    </xsl:attribute>

    <xsl:attribute name="TopMost">
      <xsl:value-of select="@isAlwaysOnTop" />
    </xsl:attribute>

  </xsl:attribute-set>

  <xsl:attribute-set name="TwoDgraphicalIndividualComponent" use-attribute-sets="TwoDgraphicalCio">

    <!--
      Champs sans correspondance ou non gérés
        - glueVertical
        - glueHorizontal
        - defaultMnemonic
        - mnemonic
        - isStrikeThrough
        - isSubScript
        - isSuperScript
        - isPreformatted
        - 
        
        
      Champs à ajouter manuellement à cause de la conversion des valeurs
      (utilisation de IF non autorisé dans attribute-set)
        - isBold (-> FontWeight)
        - isItalic (-> FontStyle)
        - isUnderlined (->FontDecorations)
    -->

    <xsl:attribute name="Width">
      <xsl:value-of select="@width" />
    </xsl:attribute>

    <xsl:attribute name="Height">
      <xsl:value-of select="@height" />
    </xsl:attribute>

    <xsl:attribute name="TopMost">
      <xsl:value-of select="@isAlwaysOnTop" />
    </xsl:attribute>

    <xsl:attribute name="FontFamily">
      <xsl:value-of select="@textFont" />
    </xsl:attribute>

    <xsl:attribute name="FontSize">
      <xsl:value-of select="@textSize" />
    </xsl:attribute>

    <xsl:attribute name="Foreground">
      <xsl:value-of select="@textColor" />
    </xsl:attribute>

  </xsl:attribute-set>

</xsl:transform>


