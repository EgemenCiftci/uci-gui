using System.Windows;
using System.Windows.Controls;
using UciGui.Models;

namespace UciGui.TemplateSelectors;

public class OptionsDataTemplateSelector : DataTemplateSelector
{
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is Option option && container is FrameworkElement fe)
        {
            switch (option.Type)
            {
                case Enums.OptionTypes.Spin:
                    return (DataTemplate)fe.FindResource("spinTemplate");
                case Enums.OptionTypes.Check:
                    return (DataTemplate)fe.FindResource("checkTemplate");
                case Enums.OptionTypes.String:
                    return (DataTemplate)fe.FindResource("stringTemplate");
                case Enums.OptionTypes.Button:
                    return (DataTemplate)fe.FindResource("buttonTemplate");
                case Enums.OptionTypes.Combo:
                    return (DataTemplate)fe.FindResource("comboTemplate");
            }
        }

        return base.SelectTemplate(item, container);
    }
}
