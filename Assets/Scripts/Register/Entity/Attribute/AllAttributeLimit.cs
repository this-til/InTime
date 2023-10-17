using System.Net.Mail;
using RegisterSystem;

namespace InTime;

public class AllAttributeLimit : RegisterManage<AttributeLimit> {
}

public class AttributeLimit : RegisterBasics {
    protected internal AttributeControl attributeControl;

    public AttributeControl getAttributeControl() => attributeControl;
}