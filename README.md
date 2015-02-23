This is my thesis project for my Management master. Catholic University Leuven worked on finding ways to help users to get used to a new UI when designers revamp their aplication. My project focused on creating a new language to specify *storyboards*. Each storyboard would show to the end users all the transitions that happened from the old UI to the new one. Full thesis [here](http://iamlookingforaninternship.com/ressources/memoire.pdf) and related paper [here](http://dl.acm.org/citation.cfm?id=1996501)

# Proof of concept

```sql
SUBSTITUTE #listbox_component_19 BY @ComboBox IN #box3 ("newComboBox") WHERE ROW 0, COL 0;
CONTRACT #newComboBox OF 90 50;
CHANGEBOX #button_1 TO #box3 WHERE ROW 0, COLINSERT 1;
SET #button_1->Content TO "GO!";
CHANGEBOX #button_0 TO #box3 WHERE ROW 0, COLINSERT 2;
SET #button_0->Content TO "[X]";
SET #label_0->FontSize TO 12;
CONTRACT #window_0 OF 40 120;
```

![](http://iamlookingforaninternship.com/images/animator.jpg)

