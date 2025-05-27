import React from 'react';
import { Editor } from '@tinymce/tinymce-react';
import { Input } from 'antd';
import { WidgetProps } from '@rjsf/utils';
import { useTheme } from './ThemeContext';

const { TextArea } = Input;

interface RichTextWidgetProps extends WidgetProps { }

const RichTextWidget: React.FC<RichTextWidgetProps> = ({ value, onChange, disabled }) => {
    const [isEditorReady, setIsEditorReady] = React.useState(false);
    const { isDarkMode } = useTheme();

    const handleEditorChange = (content: string) => {
        onChange(content);
    };

    React.useEffect(() => {
        // Check if TinyMCE script is available
        const checkTinyMCE = () => {
            if (window.tinymce) {
                setIsEditorReady(true);
            } else {
                console.warn('TinyMCE not found, using fallback textarea');
                setIsEditorReady(false);
            }
        };

        // Check immediately
        checkTinyMCE();

        // If not available, set a timeout to check again
        if (!window.tinymce) {
            const timer = setTimeout(checkTinyMCE, 1000);
            return () => clearTimeout(timer);
        }
    }, []);

    // Fallback to simple textarea if TinyMCE is not available
    if (!isEditorReady) {
        return (
            <TextArea
                value={value || ''}
                onChange={(e) => onChange(e.target.value)}
                disabled={disabled}
                rows={6}
                placeholder="Enter description (Rich text editor loading...)"
            />
        );
    }

    return (
        <Editor
            tinymceScriptSrc="/tinymce/tinymce.min.js"
            value={value || ''}
            init={{
                licenseKey: 'gpl',
                menubar: false,
                branding: false,
                elementpath: false,
                resize: true,
                height: 300,
                plugins: 'advlist autolink color lists link image charmap preview anchor searchreplace visualblocks code fullscreen insertdatetime codesample media table code wordcount',
                toolbar1: 'undo redo | formatselect bold italic underline strikethrough subscript superscript | backcolor forecolor',
                toolbar2: 'alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | codesample removeformat link',
                skin: isDarkMode ? 'oxide-dark' : 'oxide',
                content_css: isDarkMode ? 'dark' : 'default',
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
            onInit={() => setIsEditorReady(true)}
        />
    );
};

export default RichTextWidget;