import { Label, TextInput } from "flowbite-react";
import React from "react";
import { useController, UseControllerProps } from "react-hook-form";

type Props = {
    label: string;
    type?: string;
    showLabel?: boolean;
} & UseControllerProps;

export default function Input(props: Props) {
    //fiedstate is where the errors, etc are located
    const { fieldState, field } = useController({ ...props, defaultValue: "" });
    return (
        <div className="mb-3">
            {props.showLabel && (
                <div className="mb-2 block">
                    <Label htmlFor={field.name} value={props.label} />
                </div>
            )}
            <TextInput
                {...props}
                {...field}
                //type can be number/ date but default to text
                type={props.type || "text"}
                placeholder={props.label}
                //dynamic colors based on validation
                color={
                    fieldState.error
                        ? "failure"
                        : !fieldState.isDirty
                        ? ""
                        : "success"
                }
                helperText={fieldState.error?.message}
            />
        </div>
    );
}
