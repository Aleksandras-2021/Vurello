import React from 'react';
import { Editor } from '@tinymce/tinymce-react';
import { WidgetProps } from '@rjsf/utils';

interface RichTextWidgetProps extends WidgetProps { }

const RichTextWidget: React.FC<RichTextWidgetProps> = ({ value, onChange, disabled }) => {
    const handleEditorChange = (content: string) => {
        onChange(content);
    };

    return (
        <Editor
            tinymceScriptSrc="/tinymce/tinymce.min.js"
            value={value}
            init={{
                licenseKey: 'gpl',
                menubar: false,
                branding: false,
                elementpath: false,
                resize: true,
                height: 500,
                plugins: 'advlist autolink color lists link image charmap preview anchor searchreplace visualblocks code fullscreen insertdatetime codesample media table code wordcount',
                toolbar1: 'undo redo | formatselect bold italic underline strikethrough subscript superscript | backcolor forecolor',
                toolbar2: 'alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | codesample removeformat link',
                skin_url: '/tinymce/skins/ui/oxide',
                content_css: '/tinymce/skins/content/default/content.css',
                codesample_languages: [
                    { text: 'HTML', value: 'markup' },
                    { text: 'JavaScript', value: 'javascript' },
                    { text: 'CSS', value: 'css' },
                    { text: 'Python', value: 'python' },
                    { text: 'C', value: 'c' },
                    { text: 'C++', value: 'cpp' },
                    { text: 'C#', value: 'csharp' },
                ],
                disabled: disabled ?? false,
            }}
            onEditorChange={handleEditorChange}
        />
    );
};

export default RichTextWidget;
